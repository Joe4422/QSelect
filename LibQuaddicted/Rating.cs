using System;
using System.Collections.Generic;
using System.Text;

namespace LibQuaddicted
{
    public class Rating
    {
        public int? Value { get; } = null;

        public Rating(int? rating)
        {
            if (rating != null && (rating.Value <= 0 || rating.Value > 5)) throw new ArgumentException("Provided rating was not valid!");
            Value = rating;
        }

        public override string ToString()
        {
            if (Value == null)
            {
                return "Unrated";
            }
            else
            {
                return new string('☆', Value.Value); 
            }
        }
    }
}
