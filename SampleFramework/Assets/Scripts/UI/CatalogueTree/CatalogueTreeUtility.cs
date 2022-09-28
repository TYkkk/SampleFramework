﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BaseFramework
{
    public static class CatalogueTreeUtility
    {
        private static GameObject templateObject;

        public static CatalogueTreeTemplate CreateTemplate(Transform parent)
        {
            if (templateObject == null)
            {
                templateObject = Resources.Load<GameObject>("UI/CatalogueTreeTemplate");
            }

            GameObject gameObject = UnityEngine.Object.Instantiate(templateObject);
            gameObject.transform.SetParent(parent, false);
            return gameObject.GetComponent<CatalogueTreeTemplate>();
        }

        public static CatalogueTreeNode GetNodeByContent(CatalogueTreeNode treeNode, string content, bool isPrecise = true)
        {
            if (isPrecise)
            {
                if (treeNode.Content == content)
                {
                    return treeNode;
                }
            }
            else
            {
                if (treeNode.Content.Contains(content))
                {
                    return treeNode;
                }
            }

            CatalogueTreeNode result = null;

            if (treeNode.ChildNodes != null && treeNode.ChildNodes.Count > 0)
            {
                for (int i = 0; i < treeNode.ChildNodes.Count; i++)
                {
                    result = GetNodeByContent(treeNode.ChildNodes[i], content, isPrecise);
                    if (result != null)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        public static CatalogueTreeNode GetNodeByID(CatalogueTreeNode treeNode, string id)
        {
            if (treeNode.NodeID == id)
            {
                return treeNode;
            }

            CatalogueTreeNode result = null;

            if (treeNode.ChildNodes != null && treeNode.ChildNodes.Count > 0)
            {
                for (int i = 0; i < treeNode.ChildNodes.Count; i++)
                {
                    result = GetNodeByID(treeNode.ChildNodes[i], id);
                    if (result != null)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        public static CatalogueTreeNode GetLastNodeData(CatalogueTreeNode rootNode)
        {
            if (rootNode.ChildNodes != null && rootNode.ChildNodes.Count > 0)
            {
                return GetLastNodeData(rootNode.ChildNodes[rootNode.ChildNodes.Count - 1]);
            }
            else
            {
                return rootNode;
            }
        }

        public static List<CatalogueTreeNode> GetAllParents(CatalogueTreeNode targetNode, bool isDescending, bool containSelf)
        {
            List<CatalogueTreeNode> result = new List<CatalogueTreeNode>();

            if (containSelf)
            {
                result.Add(targetNode);
            }

            AddParentNodeToList(result, targetNode);

            if (isDescending)
            {
                result.Reverse();
            }

            return result;
        }

        private static void AddParentNodeToList(List<CatalogueTreeNode> result, CatalogueTreeNode target)
        {
            if (target.ParentNode != null)
            {
                result.Add(target.ParentNode);
                AddParentNodeToList(result, target.ParentNode);
            }
        }
    }
}