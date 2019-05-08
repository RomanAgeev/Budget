using System;
using Guards;

namespace Expenses.Domain {
    public abstract class Entity {
        public int Id { get; protected set; }

        bool IsTransient { get { return Id <= 0; } }

        public override bool Equals(object obj) {
            if(obj == null || GetType() != obj.GetType())
                return false;

            var entity = (Entity)obj;

            if(IsTransient || entity.IsTransient)
                return false;

            return Id == ((Entity)obj).Id;
        }

        public override int GetHashCode() {
            return IsTransient ? 0 : Id.GetHashCode() ^ 31;
        }

        public static bool operator==(Entity left, Entity right) {
            return object.Equals(left, null) ?
                object.Equals(right, null) :
                left.Equals(right);
        }

        public static bool operator!=(Entity left, Entity right) {
            return !(left == right);
        }
    }
}