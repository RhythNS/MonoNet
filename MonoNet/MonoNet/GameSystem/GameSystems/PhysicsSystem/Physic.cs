using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoNet.ECS;
using MonoNet.ECS.Components;
using MonoNet.Util;


namespace MonoNet.GameSystems.PhysicsSystem
{
    public class Physic : GameSystem
    {
        public static Physic Instance { get; private set; }

        private List<Rigidbody> rigidbodies;
        private List<Transform2> transforms;
        private List<int> avoidChecks;
  
        public Physic()
        {
            Instance = this;
            rigidbodies = new List<Rigidbody>();
            transforms = new List<Transform2>();
            avoidChecks = new List<int>();
        }

        public void Register(Rigidbody rigidbody)
        {
            if (rigidbodies.Contains(rigidbody))
            {
                Log.Warn("Rigidbobies allready contains rigidbody");
            } else
            {
                rigidbodies.Add(rigidbody);
                transforms.Add(rigidbodies[rigidbodies.Count - 1].Actor.GetComponent<Transform2>());
            }         
        }

        public void DeRegister(Rigidbody rigidbody)
        {
            if (rigidbodies.Contains(rigidbody))
            {
                rigidbodies.Remove(rigidbody);
            } else
            {
                Log.Warn("Rigidbodies does not contain this rigidbody");
            }
        }

        public override void Update(GameTime gameTime)
        {
                for (int i = 0; i < rigidbodies.Count; i++)
                {
                    transforms[i].WorldPosition = UpdatePosition(transforms[i].WorldPosition, rigidbodies[i].velocity);
                }

                for (int i = 0; i < rigidbodies.Count; i++)
                {
                    for (int j = 0; j < rigidbodies.Count; j++)
                    {
                        if (i != j && !avoidChecks.Contains(10 * j + i))
                        {
                            int collisionType = CollisionCheck(rigidbodies[i].velocity, 
                                rigidbodies[j].velocity, 
                                transforms[i],
                                transforms[j], 
                                rigidbodies[i].height, 
                                rigidbodies[j].height, 
                                rigidbodies[i].width, 
                                rigidbodies[j].width);

                            switch (collisionType)
                            {
                                case 1:
                                transforms[i].WorldPosition = new Vector2(transforms[i].WorldPosition.X , transforms[j].WorldPosition.Y - rigidbodies[j].height / 2 - rigidbodies[i].height / 2);
                                rigidbodies[i].velocity = Vector2.Zero;
                                rigidbodies[j].velocity = Vector2.Zero;
                                avoidChecks.Add(10 * i + j);
                                    break;
                                case 2:
                                transforms[i].WorldPosition = new Vector2(transforms[j].WorldPosition.X + rigidbodies[j].width / 2 + rigidbodies[i].width / 2, transforms[i].WorldPosition.Y);
                                rigidbodies[i].velocity = Vector2.Zero;
                                rigidbodies[j].velocity = Vector2.Zero;
                                avoidChecks.Add(10 * i + j);
                                    break;
                                case 3:
                                transforms[i].WorldPosition = new Vector2(transforms[i].WorldPosition.X, transforms[j].WorldPosition.Y + rigidbodies[j].height / 2 + rigidbodies[i].height / 2);
                                rigidbodies[i].velocity = Vector2.Zero;
                                rigidbodies[j].velocity = Vector2.Zero;
                                avoidChecks.Add(10 * i + j);
                                   break;
                                case 4:
                                transforms[i].WorldPosition = new Vector2(transforms[j].WorldPosition.X - rigidbodies[j].width / 2 - rigidbodies[i].width / 2, transforms[i].WorldPosition.Y);
                                rigidbodies[i].velocity = Vector2.Zero;
                                rigidbodies[j].velocity = Vector2.Zero;
                                avoidChecks.Add(10 * i + j);
                                   break;
                            }
                        }
                    }
                if (transforms[i].WorldPosition.Y + rigidbodies[i].height / 2 > 400)
                {
                    transforms[i].WorldPosition = new Vector2(transforms[i].WorldPosition.X, 400 - rigidbodies[i].height / 2);
                    rigidbodies[i].velocity.Y = 0;
                    rigidbodies[i].velocity.X *= 0.9f;
                }
                    rigidbodies[i].Actor.GetComponent<Transform2>().WorldPosition = transforms[i].WorldPosition;
                    avoidChecks.Clear();
                }     
        }

        private Vector2 UpdatePosition(Vector2 position, Vector2 velocity)
        {
            position += velocity *Time.Delta;
            return position;
        }

        /// <summary>
        /// Checks if objects collide and how
        /// </summary>
        /// <param name="velocity">moving object velocity </param>
        /// <param name="movingObj"></param>
        /// <param name="checkingObj"></param>
        /// <returns>Returns type of collision</returns>
        private int CollisionCheck(Vector2 movingVel, Vector2 checkingVel, Transform2 movingObj, Transform2 checkingObj, int heightI, int heightJ, int widthI, int widthJ)
        {
            int collisionType = 0; //0 => No collision; 1 => from aboth; 2 => from below; 3 => from left; 4 => from right 
 
            float movingBottom = (movingObj.WorldPosition.Y + heightI / 2);
            float movingTop = (movingObj.WorldPosition.Y - heightI / 2);
            float movingLeft = (movingObj.WorldPosition.X - widthI / 2);
            float movingRight = (movingObj.WorldPosition.X + widthI / 2);

            float checkingBottom = (checkingObj.WorldPosition.Y + heightJ / 2);
            float checkingTop = (checkingObj.WorldPosition.Y - heightJ / 2);
            float checkingLeft = (checkingObj.WorldPosition.X - widthJ / 2);
            float checkingRight = (checkingObj.WorldPosition.X + widthJ / 2);

            if (!((movingBottom > checkingTop && movingTop < checkingBottom) || (movingTop < checkingBottom && movingBottom > checkingTop)))
            {
                collisionType = 0;
            }
            else if (!((movingRight > checkingLeft && movingLeft < checkingRight) || (movingLeft < checkingRight && movingRight > checkingLeft)))
            {
                collisionType = 0;
            }
            else
            {
                Vector2 vector;
                vector = movingVel - checkingVel;

                if ((vector.X*vector.X) > (vector.Y*vector.Y))
                {
                    if (vector.X > 0)
                    {
                        collisionType = 4; //Left
                    } else
                    {
                        collisionType = 2; //Right
                    }
                } else
                {
                    if (vector.Y > 0)
                    {
                        collisionType = 3; //Top
                    } else
                    {
                        collisionType = 1; //Bottom
                    }
                }


            }
            return collisionType;
        }
    }
}
