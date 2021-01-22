using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace OctopusController
{
    public enum TentacleMode { LEG, TAIL, TENTACLE };

    public class MyOctopusController 
    {
        
        MyTentacleController[] _tentacles =new  MyTentacleController[4];

        Transform _currentRegion;
        Transform _target;

        Transform[] _randomTargets = new Transform[4];

        bool _done;
        readonly float _epsilon = 0.1f;
        // Max number of tries before the system gives up
        int _mtries = 10;
        // Stores the position of the randomTarget in the previous iteration
        Vector3 prevTargetPos = new Vector3(0, 0, 0);
        //Quaternions for the rotations
        Quaternion q,swing, twist;
        float shotTime;
        bool stopTentacle;
        int stoppedTentacle;
        bool letHimScore; //Bool to decide whether to let the ball pass or not

        float _twistMin, _twistMax;
        float _swingMin, _swingMax;

        #region public methods
        //DO NOT CHANGE THE PUBLIC METHODS!!

        public float TwistMin { set => _twistMin = value; }
        public float TwistMax { set => _twistMax = value; }
        public float SwingMin {  set => _swingMin = value; }
        public float SwingMax { set => _swingMax = value; }
        

        public void TestLogging(string objectName)
        {

           
            Debug.Log("Hello, I am initializing my Octopus Controller in object "+objectName);

            
        }

        public void Init(Transform[] tentacleRoots, Transform[] randomTargets)
        {
            _tentacles = new MyTentacleController[tentacleRoots.Length];

            // foreach (Transform t in tentacleRoots)
            for(int i = 0;  i  < tentacleRoots.Length; i++)
            {

                _tentacles[i] = new MyTentacleController();
                _tentacles[i].LoadTentacleJoints(tentacleRoots[i],TentacleMode.TENTACLE);
                for(int j = 0; j <_tentacles[i].Bones.Length; j++)
                {
                    _tentacles[i].Theta = new float[_tentacles[i].Bones.Length];
                    _tentacles[i].Sin = new float[_tentacles[i].Bones.Length];
                    _tentacles[i].Cos = new float[_tentacles[i].Bones.Length];
                }

                //TODO: initialize any variables needed in ccd
            }
            _twistMin = 0;
            _twistMax = 0;
            _swingMin = 0;
            _swingMax = 6;
            _randomTargets = randomTargets;
            //TODO: use the regions however you need to make sure each tentacle stays in its region

        }

              
        public void NotifyTarget(Transform target, Transform region)
        {
            switch (region.name)
            {
                case "region1":
                    stoppedTentacle = 0;
                    break;
                case "region2":
                    stoppedTentacle = 1;
                    break;
                case "region3":
                    stoppedTentacle = 2;
                    break;
                case "region4":
                    stoppedTentacle = 3;
                    break;
            }
            _currentRegion = region;
            _target = target;
        }

        public void NotifyShoot() {
            //If letHimScore is false, changes the bool to stop the ball

            shotTime = Time.time;
            Debug.Log(letHimScore);
            if (!letHimScore)
            {
                stopTentacle = true;
                letHimScore = true;
            }
        }


        public void UpdateTentacles()
        {
            //TODO: implement logic for the correct tentacle arm to stop the ball and implement CCD method
            update_ccd();
        }

        public void SetLetHimScore(bool state)
        {
            letHimScore = state;
        }


        #endregion


        #region private and internal methods
        //todo: add here anything that you need

        //Returs the decomposed swing and twist of the quaternion rotation in a specific direction 
        void decomposeSwingAndTwist(Quaternion rotation, Vector3 twistAxis, out Quaternion swing, out Quaternion twist)
        {

            //Projection of the rotation axis in the twist axis
            Vector3 p = Vector3.Project(new Vector3(rotation.x, rotation.y, rotation.z), twistAxis);

            twist = new Quaternion(p.x, p.y, p.z, rotation.w).normalized;
            swing = Quaternion.Inverse(twist) * rotation;
        }

        void update_ccd() {

            //We go through all tentacles on the list
            for(int i = 0; i < _tentacles.Length; i++)
            {

                Vector3 targetPos = _randomTargets[i].position;

                //If a shot has occurred, changes the target of the tentacle of that region and waits 3 seconds
                if (i == stoppedTentacle && stopTentacle)
                {
                    targetPos = _target.position;
                    if (Time.time > shotTime + 3f)
                    {
                        stopTentacle = false;

                    }
                }

                //Check if the CCD should be executed
                //find if the distance between the end effector and the random target is bigger than _epsilon
                if (Math.Abs(Vector3.Distance(_tentacles[i].EndEffector.transform.position, targetPos)) < _epsilon) _done = true;
                else _done = false;

                if (!_done)
                {
                    
                    if (targetPos != prevTargetPos)
                    {
                        prevTargetPos = targetPos;
                        _tentacles[i].Tries = 0;
                    }
                    // if the Max number of tries hasn't been reached
                    if (_tentacles[i].Tries <= _mtries)
                    {
                        // Starting from the second last joint (the last being the end effector)
                        // going back up to the root
                        for (int j = _tentacles[i].Bones.Length - 2; j >= 0; j--)
                        {
                            // The vector from the ith joint to the end effector
                            Vector3 r1 = _tentacles[i].EndEffector.transform.position - _tentacles[i].Bones[j].transform.position;

                            // The vector from the ith joint to the target
                            Vector3 r2 = targetPos - _tentacles[i].Bones[j].transform.position;

                            // to avoid dividing by tiny numbers
                            if (r1.magnitude * r2.magnitude <= 0.001f)
                            {
                                _tentacles[i].Cos[j] = 1;
                                _tentacles[i].Sin[j] = 0;
                            }
                            else
                            {
                                // find the components using dot and cross product
                                _tentacles[i].Cos[j] = Vector3.Dot(r1, r2) / (r1.magnitude * r2.magnitude);
                                _tentacles[i].Sin[j] = Vector3.Cross(r1, r2).magnitude / (r1.magnitude * r2.magnitude);

                            }

                            // The axis of rotation 
                            Vector3 axis = Vector3.Cross(r1, r2).normalized;

                            // find the angle between r1 and r2 (and clamp values if needed avoid errors)
                            _tentacles[i].Theta[j] = Mathf.Acos(Mathf.Clamp(_tentacles[i].Cos[j], -1, 1));

                            //Optional. correct angles if needed, depending on angles invert angle if sin component is negative
                            if (_tentacles[i].Sin[j] < 0)
                                _tentacles[i].Theta[j] = -_tentacles[i].Theta[j];

                            // Obtain an angle value between -pi and pi, and then convert to degrees
                            _tentacles[i].Theta[j] = (float)SimpleAngle(_tentacles[i].Theta[j]) * Mathf.Rad2Deg;

                            q = Quaternion.AngleAxis(_tentacles[i].Theta[j], axis);
                            decomposeSwingAndTwist(q, _tentacles[i].Bones[j].transform.up, out swing, out twist);

                            // As we access the previous joint we must make sure that is not the base, 0, to avoid errors
                            if (j != 0)
                            {

                                //Rotate the joint by the swing without constraints to easily find the rotation axis
                                _tentacles[i].Bones[j].transform.rotation = swing * _tentacles[i].Bones[j].transform.rotation;

                                //Get the angle and axis between this rotation and the parent's rotation
                                float angle = Quaternion.Angle(_tentacles[i].Bones[j].transform.rotation, _tentacles[i].Bones[j - 1].transform.rotation);
                                axis = Vector3.Cross(_tentacles[i].Bones[j - 1].up, _tentacles[i].Bones[j].up);

                                //Revert the previous rotation
                                _tentacles[i].Bones[j].transform.rotation = Quaternion.Inverse(swing) * _tentacles[i].Bones[j].transform.rotation;

                                //Apply constraints to the angle
                                angle = Mathf.Clamp(angle, _swingMin, _swingMax);
                                Quaternion constraintedSwing = Quaternion.AngleAxis(angle, axis);

                                //Rotate the joint with the constrained angle in relation to its parent
                                _tentacles[i].Bones[j].transform.rotation = constraintedSwing * _tentacles[i].Bones[j - 1].transform.rotation;

                            }
                            else
                            {
                                //Rotation without constraints
                                _tentacles[i].Bones[j].transform.rotation = swing * _tentacles[i].Bones[j].transform.rotation;
                            }
                        }

                        // Increment the CCD tries
                        _tentacles[i].Tries++;
                    }
                }
            }
        }

        // function to convert an angle to its simplest form (between -pi to pi radians)
        double SimpleAngle(double theta)
        {
            theta = theta % (2.0 * Mathf.PI);
            if (theta < -Mathf.PI)
                theta += 2.0 * Mathf.PI;
            else if (theta > Mathf.PI)
                theta -= 2.0 * Mathf.PI;
            return theta;
        }


        #endregion






    }
}
