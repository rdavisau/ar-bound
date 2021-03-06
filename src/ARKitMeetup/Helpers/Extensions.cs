﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreGraphics;
using SceneKit;
using UIKit;

namespace ARKitMeetup.Helpers
{
    public static class Extensions
    {
        private static Random _r = new Random();

        public static UIViewController GetViewController(this UIView view)
        {
            var nr = view.NextResponder;
            while (nr != null)
                if (nr as UIViewController != null)
                    return (UIViewController)nr;
                else
                    nr = nr.NextResponder;

            return null;
        }

        public static void FillWith(this UIView parent, UIView child)
        {
            child.TranslatesAutoresizingMaskIntoConstraints = false;
            parent.AddSubview(child);
            parent.AddConstraints(new[]
            {
                NSLayoutConstraint.Create(child, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, parent, NSLayoutAttribute.Leading, 1, 0),
                NSLayoutConstraint.Create(child, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, parent, NSLayoutAttribute.Trailing, 1, 0),
                NSLayoutConstraint.Create(child, NSLayoutAttribute.Top, NSLayoutRelation.Equal, parent, NSLayoutAttribute.Top, 1, 0),
                NSLayoutConstraint.Create(child, NSLayoutAttribute.Bottom, NSLayoutRelation.Equal, parent, NSLayoutAttribute.Bottom, 1, 0),
            });
        }
        
        public static UIImage Resize(this UIImage image, float toWidth) 
        {
            var canvasSize = new CGSize(toWidth, Math.Ceiling(toWidth/image.Size.Width * image.Size.Height)); 
            var rect = new CGRect(0, 0, canvasSize.Width, canvasSize.Height);
        
            UIGraphics.BeginImageContextWithOptions(canvasSize, false, image.CurrentScale);
            image.Draw(rect);
            var newImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
        
            return newImage; 
        }        

        public static void TileTexture(this SCNBox box, int num)
        {
            box.FirstMaterial.Diffuse.WrapS = SCNWrapMode.Repeat;
            box.FirstMaterial.Diffuse.WrapT = SCNWrapMode.Repeat;
            box.FirstMaterial.Diffuse.ContentsTransform = SCNMatrix4.Scale(num, num, num);
        }

        public static Task<List<TOut>> SelectToListAsync<TIn, TOut>(this IEnumerable<TIn> items,
            Func<TIn, Task<TOut>> selector)
            => items.Select(selector)
                .Results();
                
                
        public static async Task<List<T>> Results<T>(this IEnumerable<Task<T>> tasks)
        {
            var ts = await Task.WhenAll(tasks);
            return ts.ToList();
        }
                
        public static IEnumerable<T> DoEach<T>(this IEnumerable<T> objs, Action<T> action)
        {
            foreach (var obj in objs)
            {
                action(obj);
                yield return obj;
            }
        }
        
        public static TAttribute GetCustomAttribute<TAttribute>(this Type t)
            where TAttribute : Attribute
            => (TAttribute)t.GetCustomAttributes(typeof(TAttribute)).FirstOrDefault();

        public static T Random<T>(this IList<T> items)
        {
            return items[_r.Next(0, items.Count())];
        }
    }
}