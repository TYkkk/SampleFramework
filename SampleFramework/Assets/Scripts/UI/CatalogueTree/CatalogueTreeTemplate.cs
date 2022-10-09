using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BaseFramework
{
    public class CatalogueTreeTemplate : MonoBehaviour
    {
        public int AnchorsInterval = 30;

        public RectTransform MainRectTransform;

        public RectTransform ArrowRectTransform;

        public Text ContentText;

        public Button ArrowBtn;

        public Button ContentBtn;

        public RectTransform AnchorRectTransform;

        public Action<CatalogueTreeNode> SelectAction;

        public GameObject IconObj;

        public float DefaultHeight { get; private set; }

        private bool isOpen = false;

        private CatalogueTreeNode nodeData;
        private List<CatalogueTreeTemplate> loadedTemplates = new List<CatalogueTreeTemplate>();

        private Quaternion arrowCloseRotation = Quaternion.Euler(0, 0, 90);
        private Quaternion arrowOpenRotation = Quaternion.Euler(Vector3.zero);

        private CatalogueTreePanel dependPanel;

        private void Awake()
        {
            ArrowBtn.onClick.AddListener(ArrowBtnClicked);
            ContentBtn.onClick.AddListener(ContentBtnClicked);
            DefaultHeight = MainRectTransform.rect.height;
        }

        public void SetData(CatalogueTreeNode nodeData, int index, Action<CatalogueTreeNode> childClickedEvent, CatalogueTreePanel dependPanel)
        {
            ClearLoadedTemplate();

            this.nodeData = nodeData;

            this.nodeData.Template = this;

            this.dependPanel = dependPanel;

            isOpen = false;

            ContentText.text = this.nodeData.Content;

            ArrowRectTransform.localRotation = arrowCloseRotation;

            InitAnchorsRectTransformPos(index);

            gameObject.SetActive(index == 0);

            if (this.nodeData.ChildNodes == null || this.nodeData.ChildNodes.Count == 0)
            {
                ArrowRectTransform.gameObject.SetActive(false);
                IconObj.SetActive(true);
                ArrowBtn.enabled = false;
            }
            else
            {
                IconObj.SetActive(false);
                foreach (var child in this.nodeData.ChildNodes)
                {
                    var template = CatalogueTreeUtility.CreateTemplate(transform);
                    template.SetData(child, index + 1, childClickedEvent, this.dependPanel);
                    template.SelectAction = childClickedEvent;
                    if (child != null && child.Action != null)
                    {
                        template.SelectAction += child.Action;
                    }
                    loadedTemplates.Add(template);
                }
            }
        }

        private void ClearLoadedTemplate()
        {
            for (int i = 0; i < loadedTemplates.Count; i++)
            {
                Destroy(loadedTemplates[i].gameObject);
            }

            loadedTemplates.Clear();
        }

        public void InitAnchorsRectTransformPos(int index)
        {
            AnchorRectTransform.offsetMax = Vector2.zero;
            AnchorRectTransform.offsetMin = new Vector2(AnchorsInterval * index, 0);
        }

        public void AddParentSize(float size)
        {
            if (nodeData.ParentNode != null && nodeData.ParentNode.Template != null)
            {
                nodeData.ParentNode.Template.MainRectTransform.sizeDelta = new Vector2(nodeData.ParentNode.Template.MainRectTransform.sizeDelta.x, nodeData.ParentNode.Template.MainRectTransform.sizeDelta.y + size);
                nodeData.ParentNode.Template.AddParentSize(size);
            }
        }

        public void ArrowBtnClicked()
        {
            if (isOpen)
            {
                CloseNode();
            }
            else
            {
                OpenNode();
            }
        }

        public void OpenNode()
        {
            if (isOpen)
            {
                return;
            }

            foreach (var child in nodeData.ChildNodes)
            {
                child.Template.gameObject.SetActive(true);
                child.Template.AddParentSize(child.Template.MainRectTransform.sizeDelta.y);
            }

            ArrowRectTransform.localRotation = arrowOpenRotation;

            isOpen = true;
        }

        public void CloseNode()
        {
            if (!isOpen)
            {
                return;
            }

            foreach (var child in nodeData.ChildNodes)
            {
                child.Template.gameObject.SetActive(false);
                child.Template.AddParentSize(-child.Template.MainRectTransform.sizeDelta.y);
            }

            ArrowRectTransform.localRotation = arrowCloseRotation;

            isOpen = false;
        }

        private void OnDestroy()
        {
            ArrowBtn.onClick.RemoveListener(ArrowBtnClicked);
            ContentBtn.onClick.RemoveListener(ContentBtnClicked);
        }

        public void Select()
        {
            ContentText.color = new Color(0.3098039f, 0.8627451f, 0.8196079f);
        }

        public void UnSelect()
        {
            ContentText.color = Color.white;
        }

        private void ContentBtnClicked()
        {
            dependPanel.SelectNode(nodeData);
            SelectAction?.Invoke(nodeData);
        }
    }
}
