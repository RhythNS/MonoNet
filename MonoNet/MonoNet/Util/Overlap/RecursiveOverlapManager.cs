using MonoNet.Util.Datatypes;
using System;
using System.Collections.Generic;

namespace MonoNet.Util.Overlap
{
    public class RecursiveOverlapManager<T> : Overlapable, IOverlapManager<T> where T : IOverlapable
    {
        private readonly bool isLast;
        private List<T> overlapables;
        private RecursiveOverlapManager<T>[] helpers;
        private readonly Box2D box;

        public RecursiveOverlapManager(Box2D box, int layersDown)
        {
            this.box = box;
            isLast = layersDown <= 0;
            if (isLast == true)
                overlapables = new List<T>();
            else
            {
                layersDown--;
                helpers = new RecursiveOverlapManager<T>[4];
                box.width *= 0.5f;
                box.height *= 0.5f;
                helpers[0] = new RecursiveOverlapManager<T>(box, layersDown);
                box.y += box.height;
                helpers[1] = new RecursiveOverlapManager<T>(box, layersDown);
                box.x += box.width;
                helpers[2] = new RecursiveOverlapManager<T>(box, layersDown);
                box.x = this.box.x + box.width;
                box.y = this.box.y;
                helpers[3] = new RecursiveOverlapManager<T>(box, layersDown);
            }
        }
        public override Box2D GetBox() => box;

        public bool IsLast() => isLast;

        public List<T> GetListWhenLast() => overlapables;

        public List<T> GetAllOverlaps(List<T> list, T toCheck) => InnerGetOverlaps(list, toCheck.GetBox());

        public List<T> GetAllOverlaps(List<T> list, Box2D toCheck) => InnerGetOverlaps(list, toCheck);

        public List<T> GetAllOverlaps(T toCheck) => InnerGetOverlaps(new List<T>(), toCheck.GetBox());

        public List<T> GetAllOverlaps(Box2D toCheck) => InnerGetOverlaps(new List<T>(), toCheck);

        private List<T> InnerGetOverlaps(List<T> list, Box2D toCheck)
        {
            if (isLast)
            {
                for (int i = 0; i < overlapables.Count; i++)
                {
                    if (!list.Contains(overlapables[i]))
                        list.Add(overlapables[i]);
                }
            }
            else // not last
            {
                for (int i = 0; i < helpers.Length; i++)
                {
                    if (helpers[i].Overlaps(toCheck))
                        list = helpers[i].InnerGetOverlaps(list, toCheck);
                }
            }
            return list;
        }

        public List<T> GetAllOverlaps(List<T> list)
        {
            if (isLast)
            {
                list.AddRange(overlapables);
            }
            else
            {
                for (int i = 0; i < helpers.Length; i++)
                    helpers[i].GetAllOverlaps(list);
            }

            return list;
        }

        private List<T> InnerGetAllOverlaps(List<T> list)
        {
            if (isLast == false)
            {
                Array.ForEach(helpers, x => x.InnerGetAllOverlaps(list));
            }
            else // is last
            {
                for (int i = 0; i < overlapables.Count - 1; i++)
                    for (int j = i + 1; j < overlapables.Count; j++)
                        if (overlapables[i].Overlaps(overlapables[j]))
                        {
                            list.Add(overlapables[i]);
                            list.Add(overlapables[j]);
                        }
            }
            return list;
        }

        public void Add(T toAdd)
        {
            if (isLast == true)
                if (overlapables.Contains(toAdd) == true)
                    Log.Warn("OverlapHelper: Overlapable already in list! (" + toAdd + ")");
                else
                    overlapables.Add(toAdd);
            else
            {
                for (int i = 0; i < helpers.Length; i++)
                {
                    if (helpers[i].Overlaps(toAdd))
                        helpers[i].Add(toAdd);
                }
            }
        }

        public void AddAll(params T[] toAdd) => Array.ForEach(toAdd, x => Add(x));

        public void AddAll(List<T> toAdd) => toAdd.ForEach(x => Add(x));

        public void Remove(T toRemove)
        {
            if (isLast == true)
                overlapables.Remove(toRemove);
            else
            {
                for (int i = 0; i < helpers.Length; i++)
                {
                    if (helpers[i].Overlaps(toRemove))
                        helpers[i].Remove(toRemove);
                }
            }
        }

    }

}