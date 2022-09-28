using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseFramework
{
    public class CatalogueTreeNode
    {
        public string NodeID;
        public CatalogueTreeNode ParentNode;
        public List<CatalogueTreeNode> ChildNodes;
        public string Content;
        public CatalogueTreeTemplate Template;
        public Action<CatalogueTreeNode> Action;

        public CatalogueTreeNode()
        {
            NodeID = Guid.NewGuid().ToString("N");
        }

        public CatalogueTreeNode GetRootNode()
        {
            if (ParentNode != null)
            {
                return ParentNode.GetRootNode();
            }
            else
            {
                return this;
            }
        }
    }
}
