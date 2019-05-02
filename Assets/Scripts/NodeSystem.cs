﻿using System.Collections;
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

        ns.AddNode(0.0f, 0.0f, 0.0f);
        ns.AddNode(new NodeSystem.Node(10.0f, 10.0f, 10.0f));

        ns.AddLine((NodeSystem.Node)ns.Nodes[0], (NodeSystem.Node)ns.Nodes[1]);


        MonoBehaviour.print("Nodes: " + ns.Nodes.Count);
        MonoBehaviour.print("Node1 Pos: " + ((NodeSystem.Node)ns.Nodes[1]).X + ", " + ((NodeSystem.Node)ns.Nodes[1]).Y + ", " + ((NodeSystem.Node)ns.Nodes[1]).Z);
        MonoBehaviour.print("Line0 Type:" + ((NodeSystem.Line)ns.Lines[0]).Type);
        ns.RemoveNode((NodeSystem.Node)(ns.Nodes[0]));
        MonoBehaviour.print("Lines: " + ns.Lines.Count);

        _nodesystemDebug = _nodesystemDebug_temp;
    }

    //===========Node=============

    public class Node
    {
        ArrayList connections = new ArrayList();
        float x, y, z;

        public Node(float _x, float _y, float _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public void addConnection(Line connection)
        {
            connections.Add(connection);
        }

        public void addConnection(Node n1, Node n2, int _type = 0)
        {
            connections.Add(new Line(n1, n2, _type));
        }

        public ArrayList Connections
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

        public int Type
        {
            get { return type; }
            set { type = value; }
        }
    }


    //===========NODE SYSTEM=============
    ArrayList nodes = new ArrayList();
    ArrayList lines = new ArrayList();

    public NodeSystem()
    {

    }

    public void AddNode(float x, float y, float z)
    {
        nodes.Add(new Node(x, y, z));
    }

    public void AddNode(Node node)
    {
        nodes.Add(node);
    }

    public void AddLine(Line line)
    {
        lines.Add(line);
    }

    public void AddLine(Node n1, Node n2, int type = 0)
    {
        lines.Add(new Line(n1, n2, type));
    }

    public void RemoveNode(Node node)
    {
        //Get All connections to this node
        ArrayList nodeConnections = node.Connections;

        if(_nodesystemDebug)
            MonoBehaviour.print("(RemoveNode) Count: " + nodeConnections.Count);

        //Remove each connection that is connected to this node
        for (int i = 0; i < nodeConnections.Count; i++)
        {
            if (_nodesystemDebug)
                MonoBehaviour.print("(RemoveNode) i: " + i);

            Line connectionToRemove = (Line)nodeConnections[i];

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

    public ArrayList Nodes
    {
        get { return nodes; }
    }

    public ArrayList Lines
    {
        get { return lines; }
    }
}