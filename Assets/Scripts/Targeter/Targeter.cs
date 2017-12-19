using RAIN.Navigation.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Targeter
    {
        public SteeringPipeline Pipeline { get; private set; }

        public Targeter(SteeringPipeline pipeline)
        {
            Pipeline = pipeline;
        }

        public bool GetGoalPosition(out Vector3 position)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Pipeline.character.KinematicData.Arrived = false;

                //if there is a valid position
                if (this.MouseClickPosition(out position))
                {
                    //we're setting the end point
                    //this is just a small adjustment to better see the debug sphere
                    Pipeline.endDebugSphere.transform.position = position + Vector3.up;
                    Pipeline.endDebugSphere.SetActive(true);
                    //TODO: acho que isto ja nao faz nada ha seculos
                    //this.currentClickNumber = 1;
                    Pipeline.CalculatePath = true;
                }
                return true;
            }
            position = Vector3.zero;
            return false;
        }

        private bool MouseClickPosition(out Vector3 position)
        {
            RaycastHit hit;

            var ray = Pipeline.camera.ScreenPointToRay(Input.mousePosition);
            //test intersection with objects in the scene
            if (Physics.Raycast(ray, out hit))
            {
                //if there is a collision, we will get the collision point
                position = hit.point;
                return true;
            }

            position = Vector3.zero;
            //if not the point is not valid
            return false;
        }
    }
}