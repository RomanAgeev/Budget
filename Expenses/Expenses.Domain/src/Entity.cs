namespace Expenses.Domain {
    public abstract class Entity {
        int _id;

        public int Id {
            get => _id;
            protected set => _id = value;
        }

        public override bool Equals(object obj) {
            if(obj == null || GetType() != obj.GetType())
                return false;

            return _id == ((Entity)obj)._id;
        }

        public override int GetHashCode() {
            return _id.GetHashCode() ^ 31;
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