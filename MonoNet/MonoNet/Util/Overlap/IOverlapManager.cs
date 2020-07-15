using MonoNet.Util.Datatypes;
using System.Collections.Generic;

namespace MonoNet.Util.Overlap
{
    public interface IOverlapManager<T> where T : IOverlapable
    {
        List<T> GetAllOverlaps(T toCheck);

        List<T> GetAllOverlaps(Box2D toCheck);

        void Add(T toAdd);

        void AddAll(List<T> toAdd);

        void AddAll(params T[] toAdd);

        void Remove(T toRemove);
    }
}