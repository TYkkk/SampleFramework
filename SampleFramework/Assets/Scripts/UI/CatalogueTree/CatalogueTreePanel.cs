using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BaseFramework
{
    public class CatalogueTreePanel : BaseMonoBehaviour
    {
        public RectTransform ContentTransform;
        public RectTransform ScrollRectTransform;

        public List<CatalogueTreeNode> RootNodes;
        public CatalogueTreeNode CurrentSelectNode;

        public Action<CatalogueTreeNode> NodeClickedEvent;

        public bool ShowSelectFlag = true;

        public List<CatalogueTreeTemplate> LoadedTemplates = new List<CatalogueTreeTemplate>();

        public string TemplatePath = "UI/CatalogueTree/CatalogueTreeTemplate";

        private void OnDestroy()
        {
            RootNodes = null;
        }

        public void SetData(List<CatalogueTreeNode> root)
        {
            ClearLoadedTemplate();

            RootNodes = root;
            CreateTemplate();
        }

        public void SetData(CatalogueTreeNode node)
        {
            ClearLoadedTemplate();

            RootNodes = new List<CatalogueTreeNode>();
            RootNodes.Add(node);

            CreateTemplate();
        }

        public void AddNode(CatalogueTreeNode node, bool createChild)
        {
            if (node.ParentNode == null)
            {
                if (RootNodes == null)
                {
                    RootNodes = new List<CatalogueTreeNode>();
                }

                var newNode = CatalogueTreeUtility.CopyCatalogueNode(node, null, createChild);

                RootNodes.Add(newNode);

                CatalogueTreeUtility.CreateTemplate(ContentTransform, TemplatePath, newNode, 0, NodeClickedEvent, this, LoadedTemplates);
            }
            else
            {
                if (RootNodes == null)
                {
                    return;
                }

                List<CatalogueTreeNode> parentNodes = new List<CatalogueTreeNode>();
                var newNode = CatalogueTreeUtility.CopyCatalogueNode(node, null, true);
                var resultNode = CollectNeedAddNodeParent(node, ref parentNodes, newNode);

                for (int i = 0; i < parentNodes.Count; i++)
                {
                    var createNode = CatalogueTreeUtility.CopyCatalogueNode(resultNode, resultNode.ParentNode, true);
                    CatalogueTreeUtility.LinkCatalogueTreeNode(parentNodes[i], createNode);
                    parentNodes[i].Template.AddNode(createNode);
                }
            }
        }

        private CatalogueTreeNode CollectNeedAddNodeParent(CatalogueTreeNode node, ref List<CatalogueTreeNode> parentNodes, CatalogueTreeNode createNode)
        {
            GetNodesByID(RootNodes, node.ParentID, ref parentNodes);
            if (parentNodes.Count == 0 && !string.IsNullOrEmpty(node.ParentNode.ParentID))
            {
                var newNode = CatalogueTreeUtility.CopyCatalogueNode(node.ParentNode, null, false);
                CatalogueTreeUtility.LinkCatalogueTreeNode(newNode, createNode);
                return CollectNeedAddNodeParent(node.ParentNode, ref parentNodes, newNode);
            }
            else
            {
                return createNode;
            }
        }

        public void GetNodesByID(List<CatalogueTreeNode> treeNode, string id, ref List<CatalogueTreeNode> result)
        {
            foreach (var node in treeNode)
            {
                if (node.NodeID == id)
                {
                    result.Add(node);
                }

                if (node.ChildNodes != null && node.ChildNodes.Count > 0)
                {
                    GetNodesByID(node.ChildNodes, id, ref result);
                }
            }
        }

        public void CreateTemplate()
        {
            foreach (var child in RootNodes)
            {
                CatalogueTreeUtility.CreateTemplate(ContentTransform, TemplatePath, child, 0, NodeClickedEvent, this, LoadedTemplates);
            }
        }

        public void ClearLoadedTemplate()
        {
            for (int i = 0; i < LoadedTemplates.Count; i++)
            {
                Destroy(LoadedTemplates[i].gameObject);
            }

            LoadedTemplates.Clear();
        }

        public void SearchContent(string content, bool isPrecise)
        {
            if (RootNodes == null || RootNodes.Count == 0 || string.IsNullOrEmpty(content))
            {
                return;
            }

            CatalogueTreeNode result = null;
            CatalogueTreeNode rootNode = null;

            foreach (var child in RootNodes)
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

            if (RootNodes.Contains(treeNode))
            {
                foreach (var child in RootNodes)
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

        public List<CatalogueTreeTemplate> GetAllTemplate()
        {
            List<CatalogueTreeTemplate> result = new List<CatalogueTreeTemplate>();
            foreach (var child in LoadedTemplates)
            {
                CollectTemplate(child, result);
            }

            return result;
        }

        private void CollectTemplate(CatalogueTreeTemplate template, List<CatalogueTreeTemplate> collectList)
        {
            collectList.Add(template);
            if (template.LoadedTemplates != null)
            {
                foreach (var child in template.LoadedTemplates)
                {
                    CollectTemplate(child, collectList);
                }
            }
        }

        //public void SelectNode(CatalogueTreeNode node)
        //{
        //    if (CurrentSelectNode == node)
        //    {
        //        return;
        //    }

        //    if (CurrentSelectNode != null && CurrentSelectNode != node && ShowSelectFlag)
        //    {
        //        CurrentSelectNode.Template.UnSelect();
        //    }

        //    CurrentSelectNode = node;

        //    if (ShowSelectFlag)
        //    {
        //        CurrentSelectNode.Template.Select();
        //    }
        //}

        public void CloseAllNode()
        {
            List<CatalogueTreeTemplate> cacheList = new List<CatalogueTreeTemplate>();
            CollectNode(cacheList, LoadedTemplates);
            for (int i = cacheList.Count - 1; i >= 0; i--)
            {
                cacheList[i].CloseNode();
            }
        }

        private void CollectNode(List<CatalogueTreeTemplate> result, List<CatalogueTreeTemplate> target)
        {
            foreach (var child in target)
            {
                if (child.LoadedTemplates != null && child.LoadedTemplates.Count > 0)
                {
                    result.Add(child);
                    CollectNode(result, child.LoadedTemplates);
                }
            }
        }

        public void OpenLayerNode(int layer)
        {
            CloseAllNode();
            OpenLayerNode(layer, LoadedTemplates);
        }

        public void OpenLayerNode(int index, List<CatalogueTreeTemplate> target)
        {
            if (index > 0)
            {
                foreach (var child in target)
                {
                    child.OpenNode();
                    if (child.LoadedTemplates != null && child.LoadedTemplates.Count > 0)
                    {
                        index--;
                        OpenLayerNode(index, child.LoadedTemplates);
                    }
                }
            }
        }
    }
}
