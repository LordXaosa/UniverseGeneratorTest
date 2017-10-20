using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections;
using System.Collections.Specialized;
using Common;
using System.Linq;
using System.Threading.Tasks;
using UniverseGeneratorTestWpf.Views.Components;
using System.Collections.Concurrent;
using Common.Models;

namespace UniverseGeneratorTestWpf.Helpers
{
    public class GridLikeItemsSource :
        IList,
        ZoomableCanvas.ISpatialItemsSource
    {
        public Rect Extent
        {
            get
            {
                return new Rect(0, 0, (-_minX + _maxX) * _multiplier, (-_minY + _maxY) * _multiplier);
            }
        }
        List<SectorModel> list = new List<SectorModel>();
        int _minX;
        int _minY;
        int _maxX;
        int _maxY;
        int _multiplier;

        public GridLikeItemsSource(List<SectorModel> sectors, int minX, int minY, int maxX, int maxY, int multiplier)
        {
            list = sectors;
            _minX = minX;
            _minY = minY;
            _maxX = maxX;
            _maxY = maxY;
            _multiplier = multiplier;
        }

        public IEnumerable<int> Query(Rect rectangle)
        {
            rectangle.Intersect(Extent);

            var top = Math.Floor(rectangle.Top / _multiplier) - 1;
            var left = Math.Floor(rectangle.Left / _multiplier) - 1;
            var right = Math.Ceiling(rectangle.Right / _multiplier) + 1;
            var bottom = Math.Ceiling(rectangle.Bottom / _multiplier) + 1;
            var width = Math.Max(right - left, 0);
            var height = Math.Max(bottom - top, 0);
            //List<int> result = new List<int>();
            ConcurrentBag<int> result = new ConcurrentBag<int>();

            Parallel.For(0, list.Count, (i) =>
            {
                int x = -_minX + list[i].X;
                int y = -_minY + list[i].Y;
                if (x > left && x < right && y > top && y < bottom)
                    result.Add(i);
            });
            return result;
            /*for(int i = 0; i < list.Count; i++)
            {
                int x = -_minX + list[i].X;
                int y = -_minY + list[i].Y;
                if (x > left && x < right && y > top && y < bottom)
                    result.Add(i);
            }
            return result;*/
            /*List<Point> points = Quadivide(new Rect(left, top, width, height)).ToList();
            foreach (var cell in points)
            {
                var x = cell.X;
                var y = cell.Y;
                var i = x > y ?
                    Math.Pow(x, 2) + y :
                    Math.Pow(y, 2) + 2 * y - x;
                if (i < Count)
                {
                    yield return (int)i;
                }
            }*/
        }

        private IEnumerable<Point> Quadivide(Rect area)
        {
            if (area.Width > 0 && area.Height > 0)
            {
                var center = area.GetCenter();
                var x = Math.Floor(center.X);
                var y = Math.Floor(center.Y);
                yield return new Point(x, y);

                var quad1 = new Rect(area.TopLeft, new Point(x, y + 1));
                var quad2 = new Rect(area.TopRight, new Point(x, y));
                var quad3 = new Rect(area.BottomLeft, new Point(x + 1, y + 1));
                var quad4 = new Rect(area.BottomRight, new Point(x + 1, y));

                var quads = new Queue<IEnumerator<Point>>();
                quads.Enqueue(Quadivide(quad1).GetEnumerator());
                quads.Enqueue(Quadivide(quad2).GetEnumerator());
                quads.Enqueue(Quadivide(quad3).GetEnumerator());
                quads.Enqueue(Quadivide(quad4).GetEnumerator());
                while (quads.Count > 0)
                {
                    var quad = quads.Dequeue();
                    if (quad.MoveNext())
                    {
                        yield return quad.Current;
                        quads.Enqueue(quad);
                    }
                }
            }
        }

        public event EventHandler ExtentChanged;

        public event EventHandler QueryInvalidated;

        private int count;

        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
                ExtentChanged?.Invoke(this, EventArgs.Empty);
                QueryInvalidated?.Invoke(this, EventArgs.Empty);
            }
        }

        public object this[int i]
        {
            get
            {
                return list[i];
            }
            set
            {
            }
        }

        #region Irrelevant IList Members

        int IList.Add(object value)
        {
            return 0;
        }

        void IList.Clear()
        {
        }

        bool IList.Contains(object value)
        {
            return false;
        }

        int IList.IndexOf(object value)
        {
            return 0;
        }

        void IList.Insert(int index, object value)
        {
        }

        void IList.Remove(object value)
        {
        }

        void IList.RemoveAt(int index)
        {
        }

        void ICollection.CopyTo(Array array, int index)
        {
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return true; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return null; }
        }

        int ICollection.Count
        {
            get { return int.MaxValue; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield break;
        }

        #endregion
    }
}
