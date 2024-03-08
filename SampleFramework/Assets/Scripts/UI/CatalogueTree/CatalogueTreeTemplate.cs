using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BaseFramework
{
    public class CatalogueTreeTemplate : BaseMonoBehaviour
    {
        public int AnchorsInterval = 30;

        public RectTransform MainRectTransform;

        public RectTransform ArrowRectTransform;

        public Text ContentText;

        public Button ArrowBtn;

        public Button ContentBtn;

        public RectTransform AnchorRectTransform;

        public Action<CatalogueTreeNode> SelectAction;

        public GameObject SelectFlag;

        public float DefaultHeight { get; private set; }

        public List<CatalogueTreeTemplate> LoadedTemplates = new List<CatalogueTreeTemplate>();

        public bool IsSelect = false;

        public int Layer = -1;

        [HideInInspector]
        public CatalogueTreePanel DependPanel;

        private bool isOpen = false;

        private CatalogueTreeNode nodeData;

        private Quaternion arrowCloseRotation = Quaternion.Euler(0, 0, 90);
        private Quaternion arrowOpenRotation = Quaternion.Euler(Vector3.zero);

        private Action<CatalogueTreeNode> clickedEvent;

        public virtual void Awake()
        {
            ArrowBtn.onClick.AddListener(ArrowBtnClicked);
            ContentBtn.onClick.AddListener(ContentBtnClicked);
            DefaultHeight = MainRectTransform.rect.height;
        }

        public virtual void SetData(CatalogueTreeNode nodeData, int index, Action<CatalogueTreeNode> childClickedEvent, CatalogueTreePanel dependPanel)
        {
            ClearLoadedTemplate();

            this.nodeData = nodeData;

            this.nodeData.Template = this;

            this.DependPanel = dependPanel;

            Layer = index;

            clickedEvent = childClickedEvent;

            isOpen = false;

            ContentText.text = this.nodeData.Content;

            ArrowRectTransform.localRotation = arrowCloseRotation;

            InitAnchorsRectTransformPos(Layer);

            gameObject.SetActive(Layer == 0);

            if (IsSelect)
            {
                Select();
            }
            else
            {
                UnSelect();
            }

            if (this.nodeData.ChildNodes != null && this.nodeData.ChildNodes.Count > 0)
            {
                foreach (var child in this.nodeData.ChildNodes)
                {
                    CatalogueTreeUtility.CreateTemplate(MainRectTransform, this.DependPanel.TemplatePath, child, Layer + 1, clickedEvent, this.DependPanel, LoadedTemplates);
                }
            }

            UpdateArrowByChildNodesChanged();
        }

        public void AddNode(CatalogueTreeNode node)
        {
            var createTemplate = CatalogueTreeUtility.CreateTemplate(MainRectTransform, this.DependPanel.TemplatePath, node, Layer + 1, clickedEvent, this.DependPanel, LoadedTemplates);
            if (!isOpen)
            {
                UpdateArrowByChildNodesChanged();
            }
            else
            {
                createTemplate.gameObject.SetActive(true);
                createTemplate.AddParentSize(createTemplate.MainRectTransform.sizeDelta.y);
            }
        }

        private void UpdateArrowByChildNodesChanged()
        {
            ArrowRectTransform.gameObject.SetActive(nodeData.ChildNodes != null && nodeData.ChildNodes.Count > 0);
            ArrowBtn.enabled = nodeData.ChildNodes != null && nodeData.ChildNodes.Count > 0;
        }

        private void ClearLoadedTemplate()
        {
            for (int i = 0; i < LoadedTemplates.Count; i++)
            {
                Destroy(LoadedTemplates[i].gameObject);
            }

            LoadedTemplates.Clear();
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
            if (isOpen || LoadedTemplates.Count == 0)
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
            if (!isOpen || LoadedTemplates.Count == 0)
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

        public virtual void OnDestroy()
        {
            ArrowBtn.onClick.RemoveListener(ArrowBtnClicked);
            ContentBtn.onClick.RemoveListener(ContentBtnClicked);
        }

        public void Select()
        {
            SelectFlag.gameObject.SetActive(true);
            IsSelect = true;
        }

        public void UnSelect()
        {
            SelectFlag.gameObject.SetActive(false);
            IsSelect = false;
        }

        private void ContentBtnClicked()
        {
            //dependPanel.SelectNode(nodeData);
            SelectAction?.Invoke(nodeData);
        }

        public void SelectEvent()
        {
            if (IsSelect)
            {
                UnSelect();
            }
            else
            {
                Select();
            }
        }

        public CatalogueTreeNode GetNode()
        {
            return nodeData;
        }
    }
}
