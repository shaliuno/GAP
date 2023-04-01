using ImGuiNET;
using System.Reflection;

namespace Stas.GA;
public abstract class ObjectBase {
    public uint version { get; protected set; } = 0;
    /// <summary>
    /// this.Type.Name for show in debug
    /// </summary>
    protected string _tname;
    public void Set_tname(string name) {
        _tname = name;
    }
    /// <summary>
    /// this.Type.Name for show in debug
    /// </summary>
    public string tName => _tname;

    public IntPtr Address { get; protected private set; }


    public virtual void Update(nint ptr, string from) {
        Address = ptr;
    }

    public ObjectBase(IntPtr ptr, string tname = null) {
        Address = ptr;
        if (tname == null)
            _tname = "same ObjectBase...";
        else
            _tname = tname;
    }
}
/// <summary>
///     Points to a Memory location and reads/understands all the data from there.
///     CurrentAreaInstance in remote memory location changes w.r.t time or event. Due to this,
///     each remote memory object requires to implement a time/event based coroutine.
/// </summary>
public abstract class RemoteObjectBase : ObjectBase {
    public RemoteObjectBase(IntPtr ptr, string name = null) : base(ptr, name) {
        if (ptr != default)
            Tick(ptr, tName + "()");
    }
    internal abstract void Tick(IntPtr ptr, string from = null);

    /// <summary>
    ///     Knows how to clean up the object.
    /// </summary>
    protected abstract void Clear();

    #region  old ImGui
    /// <summary>
    ///     Converts the <see cref="RemoteObjectBase" /> to ImGui Widget via reflection.
    ///     By default, only knows how to convert <see cref="address" /> field
    ///     and <see cref="RemoteObjectBase" /> properties of the calling class.
    ///     For details on which specific properties are ignored read
    ///     <see cref="RemoteObjectBase.GetToImGuiMethods" /> method description.
    ///     Any other properties or fields of the derived <see cref="RemoteObjectBase" />
    ///     class should be handled by that class.
    /// </summary>
    internal virtual void ToImGui() {
        var propFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
        var properties = RemoteObjectBase.GetToImGuiMethods(this.GetType(), propFlags, this);
        ImGuiExt.IntPtrToImGui("Address", this.Address);
        foreach (var property in properties) {
            if (ImGui.TreeNode(property.Name)) {
                property.ToImGui.Invoke(property.Value, null);
                ImGui.TreePop();
            }
        }
    }

    /// <summary>
    ///     Iterates over properties of the given class via reflection
    ///     and yields the <see cref="RemoteObjectBase" /> property name and its
    ///     <see cref="RemoteObjectBase.ToImGui" /> method. Any property
    ///     that doesn't have both the getter and setter method are ignored.
    /// </summary>
    /// <param name="classType">Type of the class to traverse.</param>
    /// <param name="propertyFlags">flags to filter the class properties.</param>
    /// <param name="classObject">Class object, or null for static class.</param>
    /// <returns>Yield the <see cref="RemoteObjectBase.ToImGui" /> method.</returns>
    internal static IEnumerable<RemoteObjectPropertyDetail> GetToImGuiMethods(Type classType,
                    BindingFlags propertyFlags, object classObject) {
        var methodFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        var properties = classType.GetProperties(propertyFlags).ToList();
        for (var i = 0; i < properties.Count; i++) {
            var property = properties[i];
            if (Attribute.IsDefined(property, typeof(SkipImGuiReflection))) {
                continue;
            }

            var propertyValue = property.GetValue(classObject);
            if (propertyValue == null) {
                continue;
            }

            var propertyType = propertyValue.GetType();

            if (!typeof(RemoteObjectBase).IsAssignableFrom(propertyType)) {
                continue;
            }

            yield return new RemoteObjectPropertyDetail {
                Name = property.Name,
                Value = propertyValue,
                ToImGui = propertyType.GetMethod("ToImGui", methodFlags)
            };
        }
    }
    //public bool b_ready { get; private set; }
    //public void OnPerFrame(bool need_w8=false) {
    //    if (this.Address != IntPtr.Zero) {
    //        if(need_w8)
    //            b_ready = false;
    //        this.UpdateData(false);
    //        b_ready = true;
    //    }
    //}
    /// <summary>
    /// Attribute to put on the properties that you want to skip in <see cref="GetToImGuiMethods"/> method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    protected class SkipImGuiReflection : Attribute {
    }
    #endregion
}

