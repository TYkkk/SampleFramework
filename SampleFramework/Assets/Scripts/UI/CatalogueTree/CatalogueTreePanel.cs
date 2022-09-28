using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BaseFramework
{
    public class CatalogueTreePanel : MonoBehaviour
    {
        public RectTransform ContentTransform;
        public RectTransform ScrollRectTransform;

        private List<CatalogueTreeNode> rootNodes;
        private CatalogueTreeNode CurrentSelectNode;

        public Action<CatalogueTreeNode> RootNodeClickedEvent;
        public Action<CatalogueTreeNode> ChildNodeClickedEvent;

        public bool ShowSelectFlag = true;

        private List<CatalogueTreeTemplate> loadedTemplates = new List<CatalogueTreeTemplate>();

        private void Awake()
        {
            rootNodes = new List<CatalogueTreeNode>();
        }

        private void OnDestroy()
        {
            rootNodes = null;
        }

        public void SetData(List<CatalogueTreeNode> root)
        {
            rootNodes = root;
            CreateTemplate();
        }

        public void SetData(CatalogueTreeNode node)
        {
            rootNodes = new List<CatalogueTreeNode>();
            rootNodes.Add(node);

            CreateTemplate();
        }

        public void CreateTemplate()
        {
            ClearLoadedTemplate();

            foreach (var child in rootNodes)
            {
                var template = CatalogueTreeUtility.CreateTemplate(ContentTransform);
                template.SetData(child, 0, ChildNodeClickedEvent, this);
                template.SelectAction = RootNodeClickedEvent;

                loadedTemplates.Add(template);
            }
        }

        public void ClearLoadedTemplate()
        {
            for (int i = 0; i < loadedTemplates.Count; i++)
            {
                Destroy(loadedTemplates[i].gameObject);
            }

            loadedTemplates.Clear();
        }

        public void SearchContent(string content, bool isPrecise)
        {
            if (rootNodes == null || rootNodes.Count == 0 || string.IsNullOrEmpty(content))
            {
                return;
            }

            CatalogueTreeNode result = null;
            CatalogueTreeNode rootNode = null;

            foreach (var child in rootNodes)
            {
                result = CatalogueTreeUtility.GetNodeByContent(child, content, isPrecise);
                if (result != null)
                {
                    rootNode = child;
                    break;
                }
            }

            if (result != null && rootNode != null)
            {
                List<CatalogueTreeNode> needOpenNodes = new List<CatalogueTreeNode>();

                CheckNeedOpenParentNodes(result, needOpenNodes);

                for (int i = needOpenNodes.Count - 1; i >= 0; i--)
                {
                    needOpenNodes[i].Template.OpenNode();
                }

                StartCoroutine(DoMoveContent(result, rootNode));
            }
        }

        IEnumerator DoMoveContent(CatalogueTreeNode result, CatalogueTreeNode rootNode)
        {
            yield return new WaitForEndOfFrame();

            float moveSize = GetMoveContentSize(result) + GetSameLayerRootHigherSize(rootNode);

            if (moveSize > ScrollRectTransform.rect.height)
            {
                ContentTransform.anchoredPosition = new Vector2(ContentTransform.anchoredPosition.x, moveSize - ScrollRectTransform.rect.height);
            }
        }

        private float GetSameLayerRootHigherSize(CatalogueTreeNode treeNode)
        {
            float result = 0;

            if (rootNodes.Contains(treeNode))
            {
                foreach (var child in rootNodes)
                {
                    if (child == treeNode)
                    {
                        break;
                    }

                    result += child.Template.MainRectTransform.rect.height;
                }
            }

            return result;
        }

        private float GetMoveContentSize(CatalogueTreeNode treeNode)
        {
            float result = 0;

            if (treeNode.ParentNode == null)
            {
                return result;
            }
            else
            {
                return result + GetSameLayerHigherSize(treeNode);
            }
        }

        private float GetSameLayerHigherSize(CatalogueTreeNode treeNode)
        {
            float result = 0;

            result += treeNode.Template.DefaultHeight;

            if (treeNode.ParentNode != null)
            {
                foreach (var child in treeNode.ParentNode.ChildNodes)
                {
                    if (child == treeNode)
                    {
                        break;
                    }

                    result += child.Template.MainRectTransform.sizeDelta.y;
                }

                return result + GetSameLayerHigherSize(treeNode.ParentNode);
            }

            return result;
        }

        private void CheckNeedOpenParentNodes(CatalogueTreeNode treeNode, List<CatalogueTreeNode> result)
        {
            if (treeNode.ParentNode != null && treeNode.ParentNode.Template != null)
            {
                result.Add(treeNode.ParentNode);
                CheckNeedOpenParentNodes(treeNode.ParentNode, result);
            }
        }

        public void SelectNode(CatalogueTreeNode node)
        {
            if (CurrentSelectNode == node)
            {
                return;
            }

            if (CurrentSelectNode != null && CurrentSelectNode != node && ShowSelectFlag)
            {
                CurrentSelectNode.Template.UnSelect();
            }

            CurrentSelectNode = node;

            if (ShowSelectFlag)
            {
                CurrentSelectNode.Template.Select();
            }
        }
    }
}
