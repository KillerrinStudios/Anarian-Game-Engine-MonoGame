using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anarian.Collections
{
    public class KDTree<K, T> : IEnumerable<TreeNode<K, T>> where K : IComparable
    {
        private int count = 0;
        public int Count
        {
            get { return count; }
            private set
            {
                count = value;

                if (count < 0) count = 0;
            }
        }

        private TreeNode<K, T> root;
        public TreeNode<K, T> Root
        {
            get { return root; }
        }

        public delegate void TreeVisitFunctionDelegate(ref TreeNode<K, T> node);

        #region IEnumerable Implimentation
        public IEnumerator<TreeNode<K, T>> GetEnumerator()
        {
            var queue = new Queue<TreeNode<K, T>>();

            if (root != null)
                queue.Enqueue(root);

            for (int i = 0; i < Count; i++)
            {
                var current = queue.Dequeue();
                yield return current;

                if (current.LeftNode != null)
                    queue.Enqueue(current.LeftNode);
                if (current.RightNode != null)
                    queue.Enqueue(current.RightNode);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            // Lets call the generic version here
            return this.GetEnumerator();
        }
        #endregion


        public KDTree()
        {
            root = null;
            Count = 0;
        }

        public void Clear()
        {
            root = null;
            Count = 0;
        }
        public KDTree<K, T> Sort()
        {
            KDTree<K, T> newTree = new KDTree<K, T>();

            foreach (TreeNode<K, T> node in this)
            {
                newTree.Add(node.Key, node.Value);
            }

            return newTree;
        }

        public void Add(K key, T value)
        {
            Insert(ref root, new TreeNode<K, T>(key, value));
            count++;
        }

        public void Add(TreeNode<K, T> node)
        {
            Insert(ref root, node);
            count++;
        }

        public TreeNode<K, T> Find(K key)
        {
            return Find(key, ref root);
        }

        public TreeNode<K, T> FindValue(T _value)
        {
            return FindValue(_value, ref root);
        }

        public void Remove(K key)
        {
            //Search for the node to remove
            TreeNode<K, T> node = Find(key);
            //Search for the parent of the node to remove
            TreeNode<K, T> parent = Predecessor(key);

            //If the root node is being removed
            //Assign root to the new subtree
            if (node == parent)
            {
                Remove(ref node);
                root = node;
            }
            else
            {
                //Remove the branch or leaf
                Remove(ref node);

                //If the node is a subtree
                if (node != null) // != null added
                {
                    //Check if subtree is to the left or right
                    //Link parent to the subtree
                    if (node.Key.CompareTo(parent.Key) < 0)//node.Key < parent.Key)
                        parent.LeftNode = node;
                    else if (node.Key.CompareTo(parent.Key) > 0)//node.Key > parent.Key)
                        parent.RightNode = node;
                }
                //Check if node is null
                //Link parent to null
                else
                {
                    if (parent.LeftNode != null) // != null added
                        parent.LeftNode = node;
                    else
                        parent.RightNode = node;
                }
            }
        }
        public void RemoveValue(T _value)
        {
            Remove(FindValue(_value).Key);
        }

        //Predecessor call private helper member function
        public TreeNode<K, T> Predecessor(K key)
        {
            return Predecessor(ref root, Find(key, ref root));
        }
        public TreeNode<K, T> PredecessorValue(T _value)
        {
            return Predecessor(ref root, FindValue(_value, ref root));
        }

        //In Order traversal call private helper member function
        public void TraverseInOrder(TreeVisitFunctionDelegate visitFunction)//VisitFunction f)
        {
            VisitInOrder(ref root, visitFunction);//, f);
        }

        //Pre Order traversal call private helper member function
        public void TraversePreOrder(TreeVisitFunctionDelegate visitFunction)
        {
            VisitPreOrder(ref root, visitFunction);
        }

        //Post Order traversal call private helper member function
        public void TraversePostOrder(TreeVisitFunctionDelegate visitFunction)
        {
            VisitPostOrder(ref root, visitFunction);
        }

        //Level Order traversal call private helper member function
        public void TraverseLevelOrder(TreeVisitFunctionDelegate visitFunction)
        {
            VisitLevelOrder(ref root, visitFunction);
        }


        //Insert recursively
        private void Insert(ref TreeNode<K, T> treeNode, TreeNode<K, T> newNode)
        {
            //Insert if leaf found 
            if (treeNode == null)
                treeNode = newNode;
            //Recurse to left
            else if (newNode.Key.CompareTo(treeNode.Key) < 0) //newNode.Key < treeNode.Key)
                Insert(ref treeNode.LeftNode, newNode);
            //Recurse to right
            else
                Insert(ref treeNode.RightNode, newNode);
        }

        //Search recursively
        private TreeNode<K, T> Find(K key, ref TreeNode<K, T> node)
        {
            //If key found return node
            //If key is not found return null
            if (node == null || node.Key.Equals(key))//node.Key == key)
                return node;
            //Recurse to left
            else if (key.CompareTo(node.Key) < 0)//key < node.Key)
                return Find(key, ref node.LeftNode);
            //Recurse to right
            else if (key.CompareTo(node.Key) > 0)//key > node.Key)
                return Find(key, ref node.RightNode);
            return null;
        }

        private TreeNode<K, T> FindValue(T _value, ref TreeNode<K, T> node)
        {
            // If equal, return the node
            if (node == null || node.Value.Equals(_value))
                return node;

            TreeNode<K, T> result = null;

            // Check the left branch
            if (node.LeftNode != null)
                result = FindValue(_value, ref node.LeftNode);

            // If not, check the right branch
            if (result == null && node.RightNode != null)
                result = FindValue(_value, ref node.RightNode);

            return result;
        }

        //Find predecessor recursively
        private TreeNode<K, T> Predecessor(ref TreeNode<K, T> parent, TreeNode<K, T> node)
        {
            //Return null if no predecessor
            if (node == null)
                return null;
            //Return parent of node
            else if (parent.Key.Equals(node.Key))// == node.Key)
                return node;
            //Return leaf node
            else if (parent.LeftNode == node || parent.RightNode == node)
                return parent;
            //Recurse to left
            else if (node.Key.CompareTo(parent.Key) < 0)//node.Key < parent.Key)
                return Predecessor(ref parent.LeftNode, node);
            //Recurse to right
            else
                return Predecessor(ref parent.RightNode, node);
        }

        //Remove is recursive
        private void Remove(ref TreeNode<K, T> node)
        {
            //If node is a leaf
            //Delete and set to null
            if (node.LeftNode == null && node.RightNode == null)
            {
                TreeNode<K, T> temp = node;
                //delete temp;
                node = null;

                // Decrement the Counter
                Count--;
            }
            //If node is a right branch
            //Delete node and return right subtree
            else if (node.LeftNode == null)
            {
                TreeNode<K, T> temp = node;
                TreeNode<K, T> child = node.RightNode;

                temp.LeftNode = null;
                temp.RightNode = null;

                //delete temp;
                temp = null;

                node = child;

                // Decrement the Counter
                Count--;
            }
            //If node is a left branch
            //Delete node and return left subtree
            else if (node.RightNode == null)
            {
                TreeNode<K, T> temp = node;
                TreeNode<K, T> child = node.LeftNode;

                temp.LeftNode = null;
                temp.RightNode = null;

                //delete temp;
                temp = null;

                node = child;

                // Decrement the Counter
                Count--;
            }
            //If node is a tree
            //Copy leftmost child of right subtree and delete node recursively 
            else
            {
                // In-order predecessor (leftmost child of right subtree) 
                // Node has two children - get max of right subtree
                TreeNode<K, T> temp = node.RightNode; // get right node of the original node

                // find the lefttmost child of the subtree of the right node
                while (temp.LeftNode != null)
                    temp = temp.LeftNode;

                // copy the contents from the in-order predecessor to the original node
                node.Key = temp.Key;
                node.Value = temp.Value;

                // then delete the predecessor
                Remove(ref temp);

                // Decrement the Counter
                Count--;
            }
        }


        private List<TreeNode<K, T>> ToList()
        {
            List<TreeNode<K, T>> convertedList = new List<TreeNode<K, T>>();

            return convertedList;
        }

        private void VisitInOrder(ref TreeNode<K, T> node, TreeVisitFunctionDelegate visitFunction)
        {
            //If empty tree return NULL
            if (node == null)
                return;

            //Visit left
            VisitInOrder(ref node.LeftNode, visitFunction);

            //Return
            visitFunction(ref node);

            //Visit right
            VisitInOrder(ref node.RightNode, visitFunction);
        }

        //Traverse pre order and use functor
        private void VisitPreOrder(ref TreeNode<K, T> node, TreeVisitFunctionDelegate visitFunction)
        {
            //If empty tree return NULL
            if (node == null)
                return;

            //Return
            visitFunction(ref node);

            //Visit left
            VisitPreOrder(ref node.LeftNode, visitFunction);

            //Visit right
            VisitPreOrder(ref node.RightNode, visitFunction);
        }

        //Traverse post order and use functor
        private void VisitPostOrder(ref TreeNode<K, T> node, TreeVisitFunctionDelegate visitFunction)
        {
            //If empty tree return NULL
            if (node == null)
                return;

            //Visit left
            VisitPostOrder(ref node.LeftNode, visitFunction);

            //Visit right
            VisitPostOrder(ref node.RightNode, visitFunction);

            //Return
            visitFunction(ref node);
        }

        //Traverse level order and use functor
        private void VisitLevelOrder(ref TreeNode<K, T> node, TreeVisitFunctionDelegate visitFunction)
        {
            //Declare queue of nodes 
            //Purpose: store node sequence
            Queue<TreeNode<K, T>> nodeQueue = new Queue<TreeNode<K, T>>();

            //Declare current node
            TreeNode<K, T> currentNode = node;

            //Iterate until NULL
            while (currentNode != null)
            {
                //Call functor
                visitFunction(ref currentNode);

                //Visit left branch
                if (currentNode.LeftNode != null)
                {
                    //Enqueue node
                    nodeQueue.Enqueue(currentNode.LeftNode);
                }

                //Visit right branch
                if (currentNode.RightNode != null)
                {
                    //Enqueue node
                    nodeQueue.Enqueue(currentNode.RightNode);
                }

                //If queue is empty
                if (nodeQueue.Count != 0)
                {
                    //Get head of queue
                    currentNode = nodeQueue.Peek();
                    //Dequeue
                    nodeQueue.Dequeue();
                }
                else
                { //If end of queue set current node to null
                    currentNode = null;
                }
            }
        }
    }
}
