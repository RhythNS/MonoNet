using Microsoft.Xna.Framework;
using MonoNet.ECS.Components;
using MonoNet.Util;
using MonoNet.Util.Datatypes;
using MonoNet.Util.Overlap;
using System.Collections.Generic;
using System.Linq;

namespace MonoNet.GameSystems.PhysicsSystem
{
    public class Physic : GameSystem
    {
        public static Physic Instance { get; private set; }

        public Dictionary<MultiKey<int>, bool> collisionRules = new Dictionary<MultiKey<int>, bool>();

        private List<Rigidbody> rigidbodiesSquare;
        private List<Rigidbody> rigidbodiesCircle;
        private List<Transform2> transformsSquare;
        private List<Transform2> transformsCicle;

        private TriggerableHelper triggerableHelper;

        private RecursiveOverlapManager<Rigidbody> overlapManager;
        private List<Rigidbody> tempListOverlaps;

        public Physic()
        {
            Instance = this;
            rigidbodiesSquare = new List<Rigidbody>();
            rigidbodiesCircle = new List<Rigidbody>();
            transformsSquare = new List<Transform2>();
            transformsCicle = new List<Transform2>();
            triggerableHelper = new TriggerableHelper();

            overlapManager = new RecursiveOverlapManager<Rigidbody>(new Box2D(-100, -100, 1000, 840), 3);
            tempListOverlaps = new List<Rigidbody>(20);
        }

        public void SetLevelBox(Box2D box, int layersForOverlap)
        {
            RecursiveOverlapManager<Rigidbody> newManager = new RecursiveOverlapManager<Rigidbody>(box, layersForOverlap);
            if (overlapManager.IsLast())
            {
                newManager.AddAll(overlapManager.GetListWhenLast());
            }
            else
            {
                List<Rigidbody> allBodies = new List<Rigidbody>();
                overlapManager.GetAllOverlaps(allBodies);
                newManager.AddAll(allBodies);
            }
            overlapManager = newManager;
        }

        public void Register(Rigidbody rigidbody)
        {
            if (rigidbodiesSquare.Contains(rigidbody) && rigidbodiesCircle.Contains(rigidbody))
            {
                Log.Warn("Rigidbobies allready contains rigidbody");
            }
            else
            {
                if (rigidbody.isSquare)
                {
                    if (rigidbody.isStatic)
                    {
                        overlapManager.Add(rigidbody);
                    }
                    else
                    {
                        rigidbodiesSquare.Add(rigidbody);
                        transformsSquare.Add(rigidbodiesSquare[rigidbodiesSquare.Count - 1].Actor.GetComponent<Transform2>());
                    }
                }
                else
                {
                    rigidbodiesCircle.Add(rigidbody);
                    transformsCicle.Add(rigidbodiesCircle[rigidbodiesCircle.Count - 1].Actor.GetComponent<Transform2>());
                }
            }
        }

        public void DeRegister(Rigidbody rigidbody)
        {
            if (!rigidbodiesSquare.Contains(rigidbody) && !rigidbodiesCircle.Contains(rigidbody))
            {
                Log.Warn("Rigidbodies does not contain this rigidbody");
            }
            else
            {
                if (rigidbody.isSquare)
                {
                    if (rigidbody.isStatic)
                    {
                        overlapManager.Remove(rigidbody);
                    }
                    else
                    {
                        rigidbodiesSquare.Remove(rigidbody);
                        transformsSquare.Remove(rigidbody.Actor.GetComponent<Transform2>());
                    }
                }
                else
                {
                    rigidbodiesCircle.Remove(rigidbody);
                    transformsCicle.Remove(rigidbody.Actor.GetComponent<Transform2>());
                }
            }
        }

        public Rigidbody[] GetOverlaps(Rigidbody rigidbody)
        {
            List<Rigidbody> overlaps = new List<Rigidbody>();
            for (int i = 0; i < rigidbodiesCircle.Count; i++)
                if (rigidbodiesCircle[i] != rigidbody && rigidbodiesCircle[i].Overlaps(rigidbody))
                    overlaps.Add(rigidbodiesCircle[i]);

            for (int i = 0; i < rigidbodiesSquare.Count; i++)
                if (rigidbodiesSquare[i] != rigidbody && rigidbodiesSquare[i].Overlaps(rigidbody))
                    overlaps.Add(rigidbodiesSquare[i]);

            return overlaps.ToArray();
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < rigidbodiesSquare.Count; i++)
            {
                transformsSquare[i].WorldPosition = UpdatePosition(transformsSquare[i].WorldPosition, rigidbodiesSquare[i].velocity);
            }

            // Reset all Triggers in the TriggerHelper class
            triggerableHelper.SetAllToNonTouching();

            // Check collision for nonstatic body and nonstatic body
            for (int movingIndex = 0; movingIndex < rigidbodiesSquare.Count; movingIndex++)
            {
                rigidbodiesSquare[movingIndex].isGrounded = false;

                for (int checkingIndex = 0; checkingIndex < rigidbodiesSquare.Count; checkingIndex++)
                {
                    if (checkingIndex == movingIndex)
                        continue;

                    HandleCheckSS(rigidbodiesSquare[movingIndex], rigidbodiesSquare[checkingIndex], transformsSquare[movingIndex], transformsSquare[checkingIndex]);
                }
                // rigidbodiesSquare[movingIndex].Actor.GetComponent<Transform2>().WorldPosition = transformsSquare[movingIndex].WorldPosition;
            }

            // Check collision for nonstatic body and static body
            for (int movingIndex = 0; movingIndex < rigidbodiesSquare.Count; movingIndex++)
            {
                tempListOverlaps.Clear();
                overlapManager.GetAllOverlaps(tempListOverlaps, rigidbodiesSquare[movingIndex]);

                for (int checkingIndex = 0; checkingIndex < tempListOverlaps.Count; checkingIndex++)
                {
                    HandleCheckSS(rigidbodiesSquare[movingIndex], tempListOverlaps[checkingIndex], transformsSquare[movingIndex], tempListOverlaps[checkingIndex].Actor.GetComponent<Transform2>());
                }
            }

            for (int movingIndex = 0; movingIndex < rigidbodiesCircle.Count; movingIndex++)
            {
                for (int chekingCircles = 0; chekingCircles < rigidbodiesCircle.Count; chekingCircles++)
                {
                    bool isCollidingCC = CollisionCheckCC(rigidbodiesCircle[movingIndex], rigidbodiesCircle[chekingCircles], transformsCicle[movingIndex], transformsCicle[chekingCircles]);
                    HandleTrigger(rigidbodiesCircle[movingIndex], rigidbodiesCircle[chekingCircles]);
                }

                for (int checkingSquares = 0; checkingSquares < rigidbodiesSquare.Count; checkingSquares++)
                {
                    bool isCollidingCS = CollisionCheckCS(rigidbodiesCircle[movingIndex], rigidbodiesSquare[checkingSquares], transformsCicle[movingIndex], transformsSquare[checkingSquares]);
                    HandleTrigger(rigidbodiesCircle[movingIndex], rigidbodiesSquare[checkingSquares]);
                }
            }

            // Check to see if some triggers that were touching are not touching each other any more. If so remove them.
            triggerableHelper.RemoveAllNonTouching();
        }

        private void HandleCheckSS(Rigidbody movingBody, Rigidbody checkingBody, Transform2 movingTrans, Transform2 checkingTrans)
        {
            CollisionType collisionType = CollisionCheckSS(movingBody, checkingBody, movingTrans, checkingTrans);

            // If a collision occured, check if one of them is trigger and the other is not. If so do trigger methods and return
            if (collisionType != CollisionType.None && HandleTrigger(movingBody, checkingBody))
                return;

            // Check if collisions between the layers are allowed
            if (collisionRules.TryGetValue(new MultiKey<int>(movingBody.collisionLayer, checkingBody.collisionLayer), out bool value)
                && value == false)
                return;

            if (movingBody.IgnoreBodies != null && movingBody.IgnoreBodies.Contains(checkingBody))
                return;

            // Both are either triggers or non triggers. Continue with normal collision detection.
            switch (collisionType)
            {
                case CollisionType.None:
                    return;
                case CollisionType.Above:
                    movingTrans.WorldPosition = new Vector2(movingTrans.WorldPosition.X, checkingTrans.WorldPosition.Y - checkingBody.height / 2 - movingBody.height / 2);
                    movingBody.velocity.Y = 0;
                    movingBody.isGrounded = true;
                    break;
                case CollisionType.Below:
                    if (!movingBody.isGrounded)
                    {
                        movingTrans.WorldPosition = new Vector2(movingTrans.WorldPosition.X, checkingTrans.WorldPosition.Y + checkingBody.height / 2 + movingBody.height / 2);
                    }
                    movingBody.velocity.Y = 0;
                    checkingBody.isGrounded = true;
                    break;
                case CollisionType.Left:
                    movingTrans.WorldPosition = new Vector2(checkingTrans.WorldPosition.X - checkingBody.width / 2 - movingBody.width / 2, movingTrans.WorldPosition.Y);
                    movingBody.velocity.X = 0;
                    break;
                case CollisionType.Right:
                    movingTrans.WorldPosition = new Vector2(checkingTrans.WorldPosition.X + checkingBody.width / 2 + movingBody.width / 2, movingTrans.WorldPosition.Y);
                    movingBody.velocity.X = 0;
                    break;
            }

            movingBody.FireOnCollision(checkingBody);
        }

        /// <summary>
        /// Handles trigger methods for 2 rigidbodies. Only works if one is a trigger and the other is not a trigger.
        /// </summary>
        /// <returns>Wheter trigger methods were called.</returns>
        private bool HandleTrigger(Rigidbody movingBody, Rigidbody checkingBody)
        {
            if ((checkingBody.isTrigger == true && movingBody.isTrigger == false) ||
                (checkingBody.isTrigger == false && movingBody.isTrigger == true))
            {
                // If the Triggers were not touching each other in the prev Frame then try to add them and fire the event.
                if (triggerableHelper.Add(checkingBody, movingBody) == true)
                {
                    checkingBody.FireEventEnter(movingBody);
                    movingBody.FireEventEnter(checkingBody);
                }
                else // Triggers touched each other in the prev Frame and are still touching.
                {
                    checkingBody.FireEventStay(movingBody);
                    movingBody.FireEventStay(checkingBody);
                }

                return true;
            }
            return false;
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
            float movingHeight = movingBody.height;
            float checkingHeight = checkingBody.height;
            float movingWidth = movingBody.width;
            float checkingWidth = checkingBody.width;

            float movingBottom = (movingTrans.WorldPosition.Y + movingHeight / 2);
            float movingTop = (movingTrans.WorldPosition.Y - movingHeight / 2);
            float movingLeft = (movingTrans.WorldPosition.X - movingWidth / 2);
            float movingRight = (movingTrans.WorldPosition.X + movingWidth / 2);

            float checkingBottom = (checkingTrans.WorldPosition.Y + checkingHeight / 2);
            float checkingTop = (checkingTrans.WorldPosition.Y - checkingHeight / 2);
            float checkingLeft = (checkingTrans.WorldPosition.X - checkingWidth / 2);
            float checkingRight = (checkingTrans.WorldPosition.X + checkingWidth / 2);

            /*
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
            */
            if (movingBottom < checkingTop || movingTop > checkingBottom || movingRight < checkingLeft || movingLeft > checkingRight)
                return CollisionType.None;

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

            return distance < (circle1Rb.width + circle2Rb.width) / 2;
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
