﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Hough.Utils
{
    public static class LinqExt
    {
        public static IEnumerable<Tuple<T, T>> GetCombinationPairs<T>(this IEnumerable<T> elements)
        {
            return elements.SelectMany((elem, i) => elements.Skip(i + 1).Select(elem2 => new Tuple<T, T>(elem, elem2)));
        }
    }
}