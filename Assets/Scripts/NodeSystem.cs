using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeSystem
{
    static bool _nodesystemDebug = false;

    public static void Test()
    {
        bool _nodesystemDebug_temp = _nodesystemDebug;
        _nodesystemDebug = true;

        NodeSystem ns = new NodeSystem();

        //Add two nodes
        ns.AddNode(0.0f, 0.0f, 0.0f);
        ns.AddNode(new NodeSystem.Node(10.0f, 10.0f, 10.0f));

        if (ns.Nodes.Count != 2)
            throw new System.ArgumentException("1) Size not correct", "NodeSystem.nodes");

        //Add Connection nodes
        ns.AddLine((NodeSystem.Node)ns.Nodes[0], (NodeSystem.Node)ns.Nodes[1]);
        if (ns.Lines.Count != 1)
            throw new System.ArgumentException("2) Size not correct", "NodeSystem.lines");
        if (((Node)ns.Nodes[0]).Connections.Count != 1)
            throw new System.ArgumentException("3) Size not correct", "NodeSystem.Node[0].Connections.Count");
        if (((Node)ns.Nodes[1]).Connections.Count != 1)
            throw new System.ArgumentException("4) Size not correct", "NodeSystem.Node[1].Connections.Count");

        //Remove Node 0
        ns.RemoveNode((NodeSystem.Node)(ns.Nodes[0]));

        if (ns.Nodes.Count != 1)
            throw new System.ArgumentException("5) Size not correct", "NodeSystem.nodes");

        if (ns.Lines.Count != 0)
            throw new System.ArgumentException("6) Size not correct", "NodeSystem.lines");

        if (((Node)ns.Nodes[0]).Connections.Count != 0)
            throw new System.ArgumentException("7) Size not correct", "NodeSystem.Node[0].Connections.Count");

        _nodesystemDebug = _nodesystemDebug_temp;
    }

    //===========Node=============

    public class Node
    {
        List<Line> connections = new List<Line>();
        float x, y, z;

        //Temp variables for pathfinding
        public float hCost;
        public float gCost;
        public Node parrent;

        public float fCost
        {
            get { return hCost + gCost; }
        }

        public Node(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public List<Node> GetNeighbours(List<int> allowedTypes = null)
        {
            List<Node> neighbours = new List<Node>();

            for (int i = 0; i < connections.Count; i++)
            {
                if(allowedTypes == null || allowedTypes.Contains(connections[i].Type))
                    neighbours.Add(connections[i].GetOther(this));
                //neighbours.Add(connections[i].Nodes[1]);
            }

            return neighbours;
        }

        public void addConnection(Line connection)
        {
            connections.Add(connection);
        }

        public void addConnection(Node n1, Node n2, int _type = 0)
        {
            connections.Add(new Line(n1, n2, _type));
        }

        public List<Line> Connections
        {
            get { return connections; }
        }

        public float DistanceTo2(float _x, float _y, float _z)
        {
            return Mathf.Pow(_x - x, 2) + Mathf.Pow(_y - y, 2) + Mathf.Pow(_z - z, 2);
        }

        public float DistanceTo2(Node node)
        {
            return Mathf.Pow(node.X - x, 2) + Mathf.Pow(node.Y - y, 2) + Mathf.Pow(node.Z - z, 2);
        }

        public float DistanceTo(float _x, float _y, float _z)
        {
            return Mathf.Sqrt(DistanceTo2(_x, _y, _z));
        }

        public float DistanceTo(Node node)
        {
            return Mathf.Sqrt(DistanceTo2(node));
        }

        public float X
        {
            get { return x; }
        }
        public float Y
        {
            get { return y; }
        }
        public float Z
        {
            get { return z; }
        }
    }

    //===========Line=============

    public class Line
    {
        Node[] nodes = new Node[2];
        int type;

        public Line(Node n1, Node n2, int _type = 0)
        {
            if (n1 == n2 || n1 == null || n2 == null)
                MonoBehaviour.print("Something is wrong in Line constructor!");

            type = _type;
            nodes[0] = n1;
            nodes[1] = n2;

            nodes[0].addConnection(this);
            nodes[1].addConnection(this);
        }

        public Node[] Nodes
        {
            get { return nodes; }
        }

        public Node GetOther(Node n) {
            return (nodes[0] == n) ? nodes[1] : nodes[0];
        }

        public int Type
        {
            get { return type; }
            set { type = value; }
        }
    }

    //===========NODE SYSTEM=============
    List<Node> nodes = new List<Node>();
    List<Line> lines = new List<Line>();

    public NodeSystem()
    {

    }

    public Node AddNode(float x, float y, float z)
    {
        Node n = new Node(x, y, z);
        nodes.Add(n);

        return n;
    }

    public void AddNode(Node node)
    {
        nodes.Add(node);
    }

    public void AddLine(Line line)
    {
        lines.Add(line);
    }

    public Line AddLine(Node n1, Node n2, int type = 0)
    {
        Line line = new Line(n1, n2, type);
        lines.Add(line);

        return line;
    }

    public void RemoveNode(Node node)
    {
        //Get All connections to this node
        List<Line> nodeConnections = node.Connections;

        if(_nodesystemDebug)
            MonoBehaviour.print("(RemoveNode) Count: " + nodeConnections.Count);

        //Remove each connection that is connected to this node
        for (int i = 0; i < nodeConnections.Count; i++)
        {
            if (_nodesystemDebug)
                MonoBehaviour.print("(RemoveNode) i: " + i);

            Line connectionToRemove = nodeConnections[i];

        //For Each Connection, Remove itself from the 2 connected nodes
            connectionToRemove.Nodes[0].Connections.Remove(connectionToRemove);
            if (_nodesystemDebug)
                MonoBehaviour.print("(RemoveNode) check1");

            connectionToRemove.Nodes[1].Connections.Remove(connectionToRemove);
            if (_nodesystemDebug)
                MonoBehaviour.print("(RemoveNode) check2");

            //Remove the connection from the nodesystem
            lines.Remove(connectionToRemove);
            i--;
        }

        //Remove the Node from the node system
        nodes.Remove(node);
    }

    public void RemoveLine(Line line)
    {
        //For Each Connection, Remove itself from the 2 connected nodes
        line.Nodes[0].Connections.Remove(line);
        line.Nodes[1].Connections.Remove(line);

        //Remove the connection from the nodesystem
        lines.Remove(line);
    }

    public Node FindNodeClosestTo(float x, float y, float z)
    {
        Node n = null;
        float dist = float.MaxValue;

        for (int i = 0; i < nodes.Count; i++)
        {
            Node tempNode = ((Node)nodes[i]);
            float tempDist = tempNode.DistanceTo(x, y, z);
            if (tempDist < dist)
            {
                n = tempNode;
                dist = tempDist;
            }
        }

        return n;
    }

    public Node FindNodeClosestTo(Node _node)
    {
        Node n = null;
        float dist = float.MaxValue;

        for (int i = 0; i < nodes.Count; i++)
        {
            Node tempNode = ((Node)nodes[i]);
            if (tempNode == _node)
                continue;

            float tempDist = tempNode.DistanceTo(_node.X, _node.Y, _node.Z);
            if (tempDist < dist)
            {
                n = tempNode;
                dist = tempDist;
            }
        }

        return n;
    }

    public List<Node> Nodes
    {
        get { return nodes; }
    }

    public List<Line> Lines
    {
        get { return lines; }
    }
}
