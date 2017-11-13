namespace AjaxControlToolkit
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Web.Script.Serialization;

    internal class AnimationJavaScriptConverter : JavaScriptConverter
    {
        private static Animation Deserialize(IDictionary<string, object> obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            Animation animation = new Animation();
            if (!obj.ContainsKey("AnimationName"))
            {
                throw new InvalidOperationException("Cannot deserialize an Animation without an AnimationName property");
            }
            animation.Name = obj["AnimationName"] as string;
            foreach (KeyValuePair<string, object> pair in obj)
            {
                if ((string.Compare(pair.Key, "AnimationName", StringComparison.OrdinalIgnoreCase) != 0) && (string.Compare(pair.Key, "AnimationChildren", StringComparison.OrdinalIgnoreCase) != 0))
                {
                    animation.Properties.Add(pair.Key, pair.Value?.ToString());
                }
            }
            if (obj.ContainsKey("AnimationChildren"))
            {
                ArrayList list = obj["AnimationChildren"] as ArrayList;
                if (list == null)
                {
                    return animation;
                }
                foreach (object obj2 in list)
                {
                    IDictionary<string, object> dictionary = obj2 as IDictionary<string, object>;
                    if (obj2 != null)
                    {
                        animation.Children.Add(Deserialize(dictionary));
                    }
                }
            }
            return animation;
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type t, JavaScriptSerializer serializer)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }
            if ((t != typeof(Animation)) && !t.IsSubclassOf(typeof(Animation)))
            {
                return null;
            }
            return Deserialize(dictionary);
        }

        private static IDictionary<string, object> Serialize(Animation animation)
        {
            if (animation == null)
            {
                throw new ArgumentNullException("animation");
            }
            Dictionary<string, object> dictionary = new Dictionary<string, object> {
                ["AnimationName"] = animation.Name
            };
            foreach (KeyValuePair<string, string> pair in animation.Properties)
            {
                dictionary[pair.Key] = pair.Value;
            }
            List<IDictionary<string, object>> list = new List<IDictionary<string, object>>();
            foreach (Animation animation2 in animation.Children)
            {
                list.Add(Serialize(animation2));
            }
            dictionary["AnimationChildren"] = list.ToArray();
            return dictionary;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Animation animation = obj as Animation;
            if (animation != null)
            {
                return Serialize(animation);
            }
            return new Dictionary<string, object>();
        }

        public override IEnumerable<Type> SupportedTypes =>
            new ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(Animation) }));
    }
}

