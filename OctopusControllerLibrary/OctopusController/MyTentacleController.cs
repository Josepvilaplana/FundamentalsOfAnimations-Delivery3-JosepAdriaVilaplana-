using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;




namespace OctopusController
{

    
    internal class MyTentacleController

    //MAINTAIN THIS CLASS AS INTERNAL
    {

        TentacleMode tentacleMode;
        Transform[] _bones;
        Transform _endEffectorSphere;
        // The number of tries the CCD system is at now
        int _tries = 0;
        // Array of angles to rotate by (for each joint), as well as sin and cos values
        float[] _theta, _sin, _cos;
        // Array to store the start offsets of the tail
        Vector3[] _startOffset;
        Vector3[] _startAngle;
        Vector3[] axis;

        public Transform[] Bones { get => _bones; }
        public Transform EndEffector { get => _endEffectorSphere; }
        public int Tries { get => _tries; set => _tries = value;}
        public float[] Theta { get => _theta; set => _theta = value; }
        public float[] Sin { get => _sin; set => _sin = value; }
        public float[] Cos { get => _cos; set => _cos = value; }
        public Vector3[] StartOffset { get => _startOffset; set => _startOffset = value; }
        public Vector3[] StartAngle { get => _startAngle; set => _startAngle = value; }
        public Vector3[] Axis { get => axis; }

        void SetAxis()
        {
            axis = new Vector3[]
            {
                Vector3.forward,
                Vector3.right,
                Vector3.right,
                Vector3.right,
                Vector3.right,
                Vector3.right
            };
        }

        public void SetAngle(float angle, int i)
        {
            Bones[i].localRotation = Quaternion.Euler(Axis[i] * angle);
            if (StartAngle[i] == Vector3.forward)
            {
                Bones[i].localRotation = Quaternion.Euler(new Vector3(_startAngle[i].x, 0, 0));
            }
        }

        //Exercise 1.
        public Transform[] LoadTentacleJoints(Transform root, TentacleMode mode)
        {
            //TODO: add here whatever is needed to find the bones forming the tentacle for all modes
            //you may want to use a list, and then convert it to an array and save it into _bones
            List<Transform> jointsList = new List<Transform>();
            Transform joint = root;
            tentacleMode = mode;
            switch (tentacleMode){
                case TentacleMode.LEG:
                    //We get the first child of root because its the first joint of the chain
                    joint = joint.GetChild(0).transform;
                    while (true)
                    {
                        //If there are no more childs (it's the end effector) we save it in _endEffectorSphere
                        if (joint.childCount == 0)
                        {
                            //In _endEffectorsphere we keep a reference to the base of the leg
                            _endEffectorSphere = joint;
                            jointsList.Add(joint);
                            
                            break;
                        }
                        jointsList.Add(joint);
                        //We go to the next child. Has to be the second one, the first is the cylinder geometry
                        joint = joint.GetChild(1).transform;
                    }
                    break;
                case TentacleMode.TAIL:
                    SetAxis();
                    while (true)
                    {
                        //If there are no more childs (it's the end effector) we save it in _endEffectorSphere
                        if (joint.childCount == 0)
                        {
                            //Here we don't want to save the end_effector but the sphere,
                            //we go back to the parent and get the first child, the red sphere
                            joint = joint.parent.GetChild(0).transform;
                            _endEffectorSphere = joint;
                            break;
                        }
                        jointsList.Add(joint);
                        joint = joint.GetChild(1).transform;
                    }
                    break;
                case TentacleMode.TENTACLE:
                    //We don't want to start the chain with root or its next child (Armature)
                    //The first bone is inside Armature, so we go forward twice to get it
                    joint = joint.GetChild(0).GetChild(0).transform;
                    while (true)
                    {
                        //If there are no more childs (it's the end effector) we save it in _endEffectorSphere
                        if (joint.childCount == 0)
                        {
                            //In _endEffectorphere we keep a reference to the sphere with a collider attached to the endEffector
                            _endEffectorSphere = joint;
                            break;
                        }
                        jointsList.Add(joint);
                        joint = joint.GetChild(0).transform;
                    }
                    break;
            }
            _bones = jointsList.ToArray();
            return Bones;
        }

    }
}
