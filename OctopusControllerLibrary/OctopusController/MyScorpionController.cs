using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace OctopusController
{
  
    public class MyScorpionController
    {
        //TAIL
        Transform tailTarget;
        Transform tailBase;
        Transform tailEndEffector;
        MyTentacleController _tail;
        float tailRange;
        float DeltaGradient = 0.1f;
        float LearningRate = 30f;
        float StopThreshold = 0.1f;
        bool canShoot = false;

        //LEGS
        Transform[] legTargets;
        Transform[] legFutureBases;
        MyTentacleController[] _legs = new MyTentacleController[6];
        float animationRange;
        float[] animTime = new float[6];
        bool[] animPlaying = new bool[6];
        float animDuration = 0.1f;
        bool walking;
        Vector3[] copy;
        float[] legDistances;
        bool done;
        float treshold_condition = 0.01f;
        float maxIterations = 10;
        
        #region public
        public void InitLegs(Transform[] LegRoots,Transform[] LegFutureBases, Transform[] LegTargets)
        {
            _legs = new MyTentacleController[LegRoots.Length];
            legTargets = LegTargets;
            legFutureBases = LegFutureBases;
            //Legs init
            for (int i = 0; i < LegRoots.Length; i++)
            {
                _legs[i] = new MyTentacleController();
                _legs[i].LoadTentacleJoints(LegRoots[i], TentacleMode.LEG);
                animTime[i] = 0;
                animPlaying[i] = false;
                //TODO: initialize anything needed for the FABRIK implementation
            }
            copy = new Vector3[_legs[0].Bones.Length];
            legDistances = new float[_legs[0].Bones.Length - 1];

        }

        public void InitTail(Transform TailBase)
        {
            _tail = new MyTentacleController();
            _tail.LoadTentacleJoints(TailBase, TentacleMode.TAIL);
            tailEndEffector = _tail.EndEffector;
            tailBase = TailBase;

            //TODO: Initialize anything needed for the Gradient Descent implementation
            _tail.StartOffset = new Vector3[_tail.Bones.Length];
            _tail.StartAngle = new Vector3[_tail.Bones.Length];
            _tail.Theta = new float[_tail.Bones.Length];
            _tail.Sin = new float[_tail.Bones.Length];
            _tail.Cos = new float[_tail.Bones.Length];

            for (int i = 0; i < _tail.Bones.Length; i++)
            {
                //_tail.Theta[i] = _tail.Bones[i].transform.localEulerAngles.x * Mathf.Deg2Rad;
                _tail.Bones[i].localEulerAngles = _tail.Axis[i] * _tail.Theta[i];
                _tail.StartAngle[i] = _tail.Bones[i].transform.localEulerAngles;
                if (i != 0)
                {
                    _tail.StartOffset[i] = _tail.Bones[i].transform.position - _tail.Bones[i - 1].transform.position;
                    tailRange += _tail.StartOffset[i].magnitude;
                }
            }
        }

        //TODO: Check when to start the animation towards target and implement Gradient Descent method to move the joints.
        public void NotifyTailTarget(Transform target)
        {
            tailTarget = target;
        }

        public void CanShoot(bool shoot)
        {
            canShoot = shoot;
        }

        //TODO: Notifies the start of the walking animation
        public void NotifyStartWalk()
        {
            walking = true;
            animationRange = 5;
        }

        public void RestartBodyPosition()
        {
            //TODO: Initialize anything needed for the Gradient Descent implementation
            _tail.StartOffset = new Vector3[_tail.Bones.Length];
            _tail.StartAngle = new Vector3[_tail.Bones.Length];
            _tail.Theta = new float[_tail.Bones.Length];
            _tail.Sin = new float[_tail.Bones.Length];
            _tail.Cos = new float[_tail.Bones.Length];

            for (int i = 0; i < _tail.Bones.Length; i++)
            {
                //_tail.Theta[i] = _tail.Bones[i].transform.localEulerAngles.x * Mathf.Deg2Rad;
                _tail.Bones[i].localEulerAngles = _tail.Axis[i] * _tail.Theta[i];
                _tail.StartAngle[i] = _tail.Bones[i].transform.localEulerAngles;
                if (i != 0)
                {
                    _tail.StartOffset[i] = _tail.Bones[i].transform.position - _tail.Bones[i - 1].transform.position;
                    tailRange += _tail.StartOffset[i].magnitude;
                }
            }
        }

        //TODO: create the apropiate animations and update the IK from the legs and tail

        public void UpdateIK(float delta)
        {
            updateLegPos(delta);
            updateTail();
        }
        #endregion


        #region private
        //TODO: Implement the leg base animations and logic
        private void updateLegPos(float delta)
        {
            for (int i = 0; i < 6; i++)
            {
                if ((Vector3.Distance(_legs[i].Bones[0].transform.position, legFutureBases[i].transform.position)) > 1)
                {
                    animPlaying[i] = true;
                    //_legs[i].Bones[0].transform.position = legFutureBases[i].transform.position;
                }
                if(animPlaying[i] && (animTime[i] < animDuration))
                {
                    Debug.Log("The leg " + i + "is moving " + animTime[i]);
                    animTime[i] += delta;
                    _legs[i].Bones[0].transform.position = Vector3.Lerp(_legs[i].Bones[0].transform.position, legFutureBases[i].transform.position, animTime[i] / animDuration);
                }
                else
                {
                    animTime[i] = 0;
                    animPlaying[i] = false;
                }
            }
            updateLegs();
        }

        //TODO: implement Gradient Descent method to move tail if necessary
        private void updateTail()
        {
            if ((Vector3.Distance(tailBase.transform.position, tailTarget.transform.position) < tailRange) && canShoot)
            {
                ApproachTarget(tailTarget.transform.position);
            }
        }

        //TODO: implement fabrik method to move legs 
        private void updateLegs()
        {
            for (int i = 0; i < 6; i++)
            {
                Vector3 targetPos = legTargets[i].transform.position;
                copy[0] = _legs[i].Bones[0].transform.position;


                for (int j = 0; j < copy.Length - 1; j++)
                {
                    Vector3 temp = _legs[i].Bones[j + 1].position;
                    copy[j + 1] = temp;
                    legDistances[j] = Vector3.Distance(copy[j + 1], copy[j]); 
                }

                done = Vector3.Magnitude(copy[copy.Length - 1] - targetPos) < treshold_condition;
                if (!done)
                {
                    float targetRootDist = Vector3.Distance(copy[0], targetPos);

                    // Update joint positions
                    if (targetRootDist > legDistances.Sum())
                    {
                        // The target is unreachable
                        for (int j = 0; j < copy.Length - 1; j++)
                        {
                            float r = Vector3.Distance(targetPos, copy[j]);
                            float l = legDistances[j] / r;
                            copy[j + 1] = (1 - l) * copy[j] + l * targetPos;

                        }
                    }
                    else
                    {
                        float dif = Vector3.Distance(targetPos, copy[copy.Length - 1]);
                        int loopCount = 0;
                        // The target is reachable
                        while (dif > treshold_condition && loopCount < maxIterations)
                        {
                            // STAGE 1: FORWARD REACHING
                            copy[copy.Length - 1] = targetPos;
                            for (int j = copy.Length - 2; j > 0; j--)
                            {
                                float r = Vector3.Distance(copy[j + 1], copy[j]);
                                float l = legDistances[j] / r;
                                copy[j] = (1 - l) * copy[j + 1] + l * copy[j];


                            }

                            // STAGE 2: BACKWARD REACHING
                            for (int j = 0; j < copy.Length - 1; j++)
                            {
                                float r = Vector3.Distance(copy[j + 1], copy[j]);
                                float l = legDistances[j] / r;
                                copy[j + 1] = (1 - l) * copy[j] + l * copy[j + 1];

                            }
                            dif = Vector3.Distance(targetPos, copy[copy.Length - 1]);
                            loopCount++;
                        }
                    }

                    // Update original joint rotations
                    for (int j = 0; j <= _legs[i].Bones.Length - 2; j++)
                    {

                        Vector3 initVec = _legs[i].Bones[j + 1].position - _legs[i].Bones[j].position;
                        Vector3 actualVec = copy[j + 1] - copy[j];

                        float cos = Vector3.Dot(initVec, actualVec) / (initVec.magnitude * actualVec.magnitude);
                        float sin = Vector3.Cross(initVec, actualVec).magnitude / (initVec.magnitude * actualVec.magnitude);
                        float angle = Mathf.Atan2(sin, cos) * Mathf.Rad2Deg;

                        Vector3 axis = Vector3.Cross(initVec, actualVec).normalized;
                        _legs[i].Bones[j].rotation = Quaternion.AngleAxis(angle, axis) * _legs[i].Bones[j].rotation;

                        _legs[i].Bones[j + 1].position = new Vector3(copy[j + 1].x, copy[j + 1].y, copy[j + 1].z);
                    }
                }
            }
        }

        public void ApproachTarget(Vector3 target)
        {
            // Calculem la funció d'error del gradient (distància del target amb l'end effector)
            float error = DistanceFromTarget(target);

            // En cas de que la distància sigui major a la desitjada
            if (error > StopThreshold)
            {
                for (int i = _tail.Bones.Length - 1; i >= 0; i--)
                {
                    // Calculem el gradient parcial amb CalculateGradient
                    float gradient = CalculateGradient(target, i, DeltaGradient);
                    _tail.Theta[i] -= LearningRate * gradient;
                    // Apliquem el canvi d'angle al joint
                    _tail.SetAngle(_tail.Theta[i], i);
                }
            }

        }

        public float CalculateGradient(Vector3 target, int i, float delta)
        {
            // Saves the angle
            // it will be sotred later
            float solutionAngle = _tail.Theta[i];

            // Gradient: [F(x+h) - F(x)] / h
            // Update  : Solution -= LearningRate * Gradient
            float f_x = DistanceFromTarget(target);

            _tail.Theta[i] += delta;
            float f_x_plus_h = DistanceFromTarget(target);

            float gradient = (f_x_plus_h - f_x) / delta;

            // Restores
            _tail.Theta[i] = solutionAngle;

            return gradient;
        }

        // Returns the distance from the target, given a solution
        public float DistanceFromTarget(Vector3 target)
        {
            Vector3 point = ForwardKinematics();
            return Vector3.Distance(point, target);
        }

        // Simulates the forward kinematics, given a solution.
        private Vector3 ForwardKinematics()
        {
            Vector3 prevPoint = _tail.Bones[0].transform.position;

            // Takes object initial rotation into account
            Quaternion rotation = tailBase.transform.rotation;
            //Quaternion rotation = Quaternion.Euler(_tail.StartAngle[0]);
            for (int i = 1; i < _tail.Bones.Length; i++)
            {
                //Rotates around a new axis
                rotation *= Quaternion.AngleAxis(_tail.Theta[i - 1], _tail.Axis[i-1]);
                if (_tail.Axis[i - 1] == Vector3.forward)
                {
                    rotation *= Quaternion.AngleAxis(_tail.StartAngle[i - 1].x, Vector3.right);
                }
                Vector3 nextPoint = prevPoint + rotation * _tail.StartOffset[i];
                prevPoint = nextPoint;
            }
            // The end of the effector
            return prevPoint;
        }

        #endregion
    }
}
