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

        private List<Rigidbody> rigidbodiesSquare;
        private List<Rigidbody> rigidbodiesCircle;
        private List<Transform2> transformsSquare;
        private List<Transform2> transformsCicle;

        public Physic()
        {
            Instance = this;
            rigidbodiesSquare = new List<Rigidbody>();
            rigidbodiesCircle = new List<Rigidbody>();
            transformsSquare = new List<Transform2>();
            transformsCicle = new List<Transform2>();
        }

        public void Register(Rigidbody rigidbody)
        {
            if (rigidbodiesSquare.Contains(rigidbody) || rigidbodiesCircle.Contains(rigidbody))
            {
                Log.Warn("Rigidbobies allready contains rigidbody");
            }
            else
            {
                if (rigidbody.isSquare)
                {
                    rigidbodiesSquare.Add(rigidbody);
                    transformsSquare.Add(rigidbodiesSquare[rigidbodiesSquare.Count - 1].Actor.GetComponent<Transform2>());
                } else
                {
                    rigidbodiesCircle.Add(rigidbody);
                    transformsCicle.Add(rigidbodiesCircle[rigidbodiesCircle.Count - 1].Actor.GetComponent<Transform2>());
                }
            }
        }

        public void DeRegister(Rigidbody rigidbody)
        {
            if (!rigidbodiesSquare.Contains(rigidbody) || !rigidbodiesCircle.Contains(rigidbody))
            {
                Log.Warn("Rigidbodies does not contain this rigidbody");
            }
            else
            {
                if (rigidbody.isSquare)
                {
                    rigidbodiesSquare.Remove(rigidbody);
                } else
                {
                    rigidbodiesCircle.Remove(rigidbody);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < rigidbodiesSquare.Count; i++)
            {
                transformsSquare[i].WorldPosition = UpdatePosition(transformsSquare[i].WorldPosition, rigidbodiesSquare[i].velocity);
            }

            for (int movingIndex = 0; movingIndex < rigidbodiesSquare.Count; movingIndex++)
            {
                if (rigidbodiesSquare[movingIndex].isStatic)
                    return;

                for (int checkingIndex = movingIndex + 1; checkingIndex < rigidbodiesSquare.Count; checkingIndex++)
                {
                    CollisionType collisionType = CollisionCheckSS(rigidbodiesSquare[movingIndex], rigidbodiesSquare[checkingIndex], transformsSquare[movingIndex], transformsSquare[checkingIndex]);

                    switch (collisionType)
                    {
                        case CollisionType.None:
                            rigidbodiesSquare[movingIndex].grounded = false;
                            continue; // no collision happend
                        case CollisionType.Above:
                            // case CollisionType.Below:
                            transformsSquare[movingIndex].WorldPosition = new Vector2(transformsSquare[movingIndex].WorldPosition.X, transformsSquare[checkingIndex].WorldPosition.Y - rigidbodiesSquare[checkingIndex].height / 2 - rigidbodiesSquare[movingIndex].height / 2);
                            rigidbodiesSquare[movingIndex].velocity.Y = 0;
                            rigidbodiesSquare[movingIndex].grounded = true;
                            break;
                        case CollisionType.Below:
                            // case CollisionType.Right:
                            transformsSquare[movingIndex].WorldPosition = new Vector2(transformsSquare[checkingIndex].WorldPosition.X + rigidbodiesSquare[checkingIndex].width / 2 + rigidbodiesSquare[movingIndex].width / 2, transformsSquare[movingIndex].WorldPosition.Y);
                            rigidbodiesSquare[movingIndex].velocity.Y = 0;
                            break;
                        case CollisionType.Left:
                            // case CollisionType.Above:
                            transformsSquare[movingIndex].WorldPosition = new Vector2(transformsSquare[movingIndex].WorldPosition.X, transformsSquare[checkingIndex].WorldPosition.Y - rigidbodiesSquare[checkingIndex].height / 2 - rigidbodiesSquare[movingIndex].height / 2);
                            rigidbodiesSquare[movingIndex].velocity.X = 0;
                            break;
                        case CollisionType.Right:
                            // ase CollisionType.Left:
                            transformsSquare[movingIndex].WorldPosition = new Vector2(transformsSquare[checkingIndex].WorldPosition.X + rigidbodiesSquare[checkingIndex].width / 2 + rigidbodiesSquare[movingIndex].width / 2, transformsSquare[movingIndex].WorldPosition.Y);
                            rigidbodiesSquare[movingIndex].velocity.X = 0;
                            break;
                    }
                }

                if (transformsSquare[movingIndex].WorldPosition.Y + rigidbodiesSquare[movingIndex].height / 2 > 400)
                {
                    transformsSquare[movingIndex].WorldPosition = new Vector2(transformsSquare[movingIndex].WorldPosition.X, 400 - rigidbodiesSquare[movingIndex].height / 2);
                    rigidbodiesSquare[movingIndex].velocity.X *= 0.95f;
                    rigidbodiesSquare[movingIndex].velocity.Y = 0;
                }
                rigidbodiesSquare[movingIndex].Actor.GetComponent<Transform2>().WorldPosition = transformsSquare[movingIndex].WorldPosition;
            }

            for (int movingIndex = 0; movingIndex < rigidbodiesCircle.Count; movingIndex++)
            {
                for (int chekingCircles = 0; chekingCircles < rigidbodiesCircle.Count; chekingCircles++)
                {
                    bool isCollidingCC = CollisionCheckCC(rigidbodiesCircle[movingIndex], rigidbodiesCircle[chekingCircles], transformsCicle[movingIndex], transformsCicle[chekingCircles]);
                    //TODO Trigger
                }

                for (int checkingSquares = 0; checkingSquares < rigidbodiesSquare.Count; checkingSquares++)
                {
                    bool isCollidingCS = CollisionCheckCS(rigidbodiesCircle[movingIndex], rigidbodiesSquare[checkingSquares], transformsCicle[movingIndex], transformsSquare[checkingSquares]);
                    //TODO Trigger
                }
            }
        }

        private Vector2 UpdatePosition(Vector2 position, Vector2 velocity)
        {
            position += velocity * Time.Delta;
            return position;
        }

        private enum CollisionType
        {
            None = 0, Above = 1, Below = 2, Left = 3, Right = 4
        }

        /// <summary>
        /// Checks if objects collide and how
        /// </summary>
        /// <param name="velocity">moving object velocity </param>
        /// <param name="movingTrans"></param>
        /// <param name="checkingTrans"></param>
        /// <returns>Returns type of collision</returns>
        private CollisionType CollisionCheckSS(Rigidbody movingBody, Rigidbody checkingBody, Transform2 movingTrans, Transform2 checkingTrans)
        {
            Vector2 movingVel = movingBody.velocity;
            Vector2 checkingVel = checkingBody.velocity;
            int movingHeight = movingBody.height;
            int checkingHeight = checkingBody.height;
            int movingWidth = movingBody.width;
            int checkingWidth = checkingBody.width;

            float movingBottom = (movingTrans.WorldPosition.Y + movingHeight / 2);
            float movingTop = (movingTrans.WorldPosition.Y - movingHeight / 2);
            float movingLeft = (movingTrans.WorldPosition.X - movingWidth / 2);
            float movingRight = (movingTrans.WorldPosition.X + movingWidth / 2);

            float checkingBottom = (checkingTrans.WorldPosition.Y + checkingHeight / 2);
            float checkingTop = (checkingTrans.WorldPosition.Y - checkingHeight / 2);
            float checkingLeft = (checkingTrans.WorldPosition.X - checkingWidth / 2);
            float checkingRight = (checkingTrans.WorldPosition.X + checkingWidth / 2);

            bool one = (movingBottom > checkingTop && movingTop < checkingBottom);
            bool two = (movingTop < checkingBottom && movingBottom > checkingTop);
            bool three = (movingRight > checkingLeft && movingLeft < checkingRight);
            bool four = (movingLeft < checkingRight && movingRight > checkingLeft);

            if (!one && !two)
            {
                return CollisionType.None;
            }
            else if (!three && !four)
            {
                return CollisionType.None;
            }

            return CollisionSide(checkingTrans.WorldPosition, movingTrans.WorldPosition, checkingBody.width, checkingBody.height);
        }

        /// <summary>
        /// Checks if two circles collide with eachother
        /// </summary>
        /// <param name="circle1Rb">circle 1 rigidbody</param>
        /// <param name="circle2Rb">circle 2 rigidbody </param>
        /// <param name="circle1Trans">circle 1 transform </param>
        /// <param name="circle2Trans">circle 2 transform </param>
        /// <returns></returns>
        private bool CollisionCheckCC(Rigidbody circle1Rb, Rigidbody circle2Rb, Transform2 circle1Trans, Transform2 circle2Trans)
        {
            float distance = Vector2.Distance(circle2Trans.WorldPosition, circle1Trans.WorldPosition);

            if (distance < (circle1Rb.width + circle2Rb.width) / 2)
            {
                return true;
            } else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a circle collides with a square
        /// </summary>
        /// <param name="circleRb">circle rigidbody </param>
        /// <param name="squareRb">square rigidbody </param>
        /// <param name="circleTrans">circle transform </param>
        /// <param name="squareTrans">square transform </param>
        /// <returns>collision(ture) no collision(false) </returns>
        private bool CollisionCheckCS(Rigidbody circleRb, Rigidbody squareRb, Transform2 circleTrans, Transform2 squareTrans)
        {
            float distance = Vector2.Distance(squareTrans.WorldPosition, circleTrans.WorldPosition);
            CollisionType cType = CollisionSide(squareTrans.WorldPosition, circleTrans.WorldPosition, squareRb.width, squareRb.height);
            
            switch (cType)
            {
                case CollisionType.Above:
                    return distance < (circleRb.width + squareRb.height) / 2;

                case CollisionType.Below:
                    return distance < (circleRb.width + squareRb.height) / 2;
                    
                case CollisionType.Left:
                    return distance < (circleRb.width + squareRb.width) / 2;
      
                case CollisionType.Right:
                    return distance < (circleRb.width + squareRb.width) / 2;
                default:
                    return false;
            }
        }

        /// <summary>
        /// returns the side of the collision
        /// </summary>
        /// <param name="position1">position of checking object </param>
        /// <param name="position2">position of moving object </param>
        /// <param name="width">width of checking object</param>
        /// <param name="height">height of checking object</param>
        /// <returns>returns the side of the collision</returns>
        private CollisionType CollisionSide(Vector2 position1, Vector2 position2, float width, float height)
        {
            Vector2 vector = position1 - position2;

            if (vector.X * vector.X * width > vector.Y * vector.Y * height)
            {
                if (vector.X > 0)
                {
                    return CollisionType.Left;
                }
                else
                {
                    return CollisionType.Right;
                }
            }
            else if (vector.Y > 0)
            {
                return CollisionType.Above;
            }
            else
            {
                return CollisionType.Below;
            }
        }
    }
}
