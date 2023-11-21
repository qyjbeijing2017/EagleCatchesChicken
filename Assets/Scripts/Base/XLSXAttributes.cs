using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class XLSXClassAttribute : Attribute
{
    public virtual bool isReadOnly => false;
    public virtual bool isWriteOnly => false;
}

public abstract class IXLSXFiledAttribute : Attribute
{
    public virtual bool isReadOnly => false;
    public virtual bool isWriteOnly => false;
    public virtual Func<object, string> writer { get; protected set; }
    public virtual Func<string, object> reader { get; protected set; }
    public virtual HashSet<Type> referenceTypes { get; protected set; }
}

[AttributeUsage(AttributeTargets.Class)]
public class XLSXLocalAttribute : XLSXClassAttribute { }

[AttributeUsage(AttributeTargets.Field)]
public class XLSXReadOnlyAttribute : IXLSXFiledAttribute
{
    public override bool isReadOnly => true;
}

[AttributeUsage(AttributeTargets.Field)]
public class XLSXWriteOnlyAttribute : IXLSXFiledAttribute
{
    public override bool isWriteOnly => true;
}

[AttributeUsage(AttributeTargets.Field)]
public class XLSXIgnoreAttribute : IXLSXFiledAttribute
{
    public override bool isReadOnly => true;
    public override bool isWriteOnly => true;
}

[AttributeUsage(AttributeTargets.Field)]
public class XLSXReaderAttribute : IXLSXFiledAttribute
{
    public XLSXReaderAttribute(Func<string, object> reader)
    {
        this.reader = reader;
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class XLSXWriterAttribute : IXLSXFiledAttribute
{
    public XLSXWriterAttribute(Func<object, string> writer)
    {
        this.writer = writer;
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class XLSXBehaviorToScriptableObject : IXLSXFiledAttribute
{
    public XLSXBehaviorToScriptableObject(string configFolder)
    {
#if UNITY_EDITOR
        reader = (value) =>
        {
            var type = Type.GetType(value);
            var asset = AssetDatabase.LoadAssetAtPath($"{configFolder}/{type.Name.Replace("ScriptableObject", "")}_{value}.asset", type);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance(type);
                AssetDatabase.CreateAsset(asset, $"{configFolder}/{type.Name.Replace("ScriptableObject", "")}_{value}.asset");
                AssetDatabase.SaveAssets();
            }
            return asset;
        };
#endif
    }
}

public class XLSXTools
{
    public static string TypeToString(Type t, object instance)
    {

        if (t == typeof(Vector2))
        {
            var value = (Vector2)instance;
            return $"{value.x},{value.y}";
        }
        else if (t == typeof(Vector3))
        {
            var value = (Vector3)instance;
            return $"{value.x},{value.y},{value.z}";
        }
        else if (t == typeof(Vector4))
        {
            var value = (Vector4)instance;
            return $"{value.x},{value.y},{value.z},{value.w}";
        }
        else if (t == typeof(Quaternion))
        {
            var value = (Quaternion)instance;
            return $"{value.x},{value.y},{value.z},{value.w}";
        }
        else if (t == typeof(Color))
        {
            var value = (Color)instance;
            return $"{value.r},{value.g},{value.b},{value.a}";
        }
        else if (t == typeof(Color32))
        {
            var value = (Color32)instance;
            return $"{value.r},{value.g},{value.b},{value.a}";
        }
        else if (typeof(ScriptableObject).IsAssignableFrom(t))
        {
            return ((ScriptableObject)instance).name.Split('_')[1];
        }
        else if (typeof(System.Collections.IList).IsAssignableFrom(t))
        {
            var list = (System.Collections.IList)instance;
            var str = "";
            for (int i = 0; i < list.Count; i++)
            {
                var value = list[i];
                if (value != null)
                    str += TypeToString(list[i].GetType(), list[i]);
                if (i < list.Count - 1)
                {
                    str += "|";
                }
            }
            Debug.Log(str);
            return str;
        }
        else if (typeof(Enum).IsAssignableFrom(t))
        {
            return ((Enum)instance).ToString();
        }
        else if (t == typeof(string))
        {
            return (string)instance;
        }
        else if (t == typeof(int))
        {
            return ((int)instance).ToString();
        }
        else if (t == typeof(float))
        {
            return ((float)instance).ToString();
        }
        else if (t == typeof(bool))
        {
            return ((bool)instance).ToString();
        }
        else if (t == typeof(long))
        {
            return ((long)instance).ToString();
        }
        else if (instance == null)
        {
            return "";
        }

        return JsonUtility.ToJson(instance);
    }

    public static object StringToType(Type t, string value, string configFolder = "")
    {

        if (t == typeof(Vector2))
        {
            var values = value.Split(',');
            return new Vector2(float.Parse(values[0]), float.Parse(values[1]));
        }
        else if (t == typeof(Vector3))
        {
            var values = value.Split(',');
            return new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
        }
        else if (t == typeof(Vector4))
        {
            var values = value.Split(',');
            return new Vector4(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
        }
        else if (t == typeof(Quaternion))
        {
            var values = value.Split(',');
            return new Quaternion(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
        }
        else if (t == typeof(Color))
        {
            var values = value.Split(',');
            return new Color(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
        }
        else if (t == typeof(Color32))
        {
            var values = value.Split(',');
            return new Color32(byte.Parse(values[0]), byte.Parse(values[1]), byte.Parse(values[2]), byte.Parse(values[3]));
        }
        else if (typeof(ScriptableObject).IsAssignableFrom(t))
        {
            return AssetDatabase.LoadAssetAtPath($"{configFolder}/{t.Name.Replace("ScriptableObject", "")}_{value}.asset", t);
        }
        else if (typeof(System.Collections.IList).IsAssignableFrom(t))
        {
            var list = (System.Collections.IList)Activator.CreateInstance(t);
            var values = value.Split('|');
            if (values.Length == 1 && values[0] == "")
            {
                return list;
            }
            for (int i = 0; i < values.Length; i++)
            {
                list.Add(StringToType(t.GetGenericArguments()[0], values[i], configFolder));
            }
            return list;
        }
        else if (typeof(Enum).IsAssignableFrom(t))
        {
            return Enum.Parse(t, value);
        }
        else if (t == typeof(string))
        {
            return value;
        }
        else if (t == typeof(int))
        {
            return int.Parse(value);
        }
        else if (t == typeof(float))
        {
            return float.Parse(value);
        }
        else if (t == typeof(bool))
        {
            return bool.Parse(value);
        }
        else if (t == typeof(long))
        {
            return long.Parse(value);
        }
        else if (value == "")
        {
            return null;
        }

        return JsonUtility.FromJson(value, t);
    }

    public static List<Type> AllTypes()
    {
        var types = typeof(ScriptableObject)
            .Assembly
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(ScriptableObject)))
            .ToList();

        types.Sort((a, b) =>
        {
            foreach (var field in a.GetFields())
            {
                if (b.IsAssignableFrom(field.FieldType))
                {
                    return 1;
                }
                if(field.FieldType.IsGenericType)
                {
                    if(b.GetGenericArguments().Contains(field.FieldType)) return 1;
                }

                field.GetCustomAttributes<IXLSXFiledAttribute>();
                foreach (var attribute in field.GetCustomAttributes<IXLSXFiledAttribute>())
                {
                    if (attribute.referenceTypes.Contains(b)) return 1;
                }
            }

            foreach (var field in b.GetFields())
            {
                if (a.IsAssignableFrom(field.FieldType))
                {
                    return -1;
                }
                if(field.FieldType.IsGenericType)
                {
                    if(a.GetGenericArguments().Contains(field.FieldType)) return -1;
                }

                field.GetCustomAttributes<IXLSXFiledAttribute>();
                foreach (var attribute in field.GetCustomAttributes<IXLSXFiledAttribute>())
                {
                    if (attribute.referenceTypes.Contains(a)) return -1;
                }
            }

            return 0;
        });

        return types;
    }
}