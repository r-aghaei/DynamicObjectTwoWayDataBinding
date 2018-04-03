using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicObjectTwoWayDataBinding
{
    public class MyCustomObject : DynamicObject, ICustomTypeDescriptor, INotifyPropertyChanged
    {
        private readonly Object thisLock = new Object();
        public event PropertyChangedEventHandler PropertyChanged;
        private Dictionary<string, object> dictionary = new Dictionary<string, object>();
    ISynchronizeInvoke syncronzeInvoke;
    public MyCustomObject(ISynchronizeInvoke value = null)
    {
        syncronzeInvoke = value;
    }
        public object this[string name]
        {
            get
            {
                lock (thisLock) { return dictionary[name]; }
            }
            set
            {
                object oldValue = null;
                lock (thisLock)
                {
                    if (dictionary.ContainsKey(name))
                        oldValue = dictionary[name];
                    dictionary[name] = value;
                }
                if (oldValue != value)
                    OnPropertyChanged(name);
            }
        }
    private void OnPropertyChanged(string name)
    {
        if (syncronzeInvoke != null && syncronzeInvoke.InvokeRequired)
            syncronzeInvoke.Invoke(new Action(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }), null);
        else
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this[binder.Name];
            return true;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this[binder.Name] = value;
            return true;
        }
        public string GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }
        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }
        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }
        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }
        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }
        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return dictionary;
        }
        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }
        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }
        public PropertyDescriptor GetDefaultProperty()
        {
            return null;
        }
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return this.GetProperties(new Attribute[] { });
        }
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            var properties = new List<MyCustomPropertyDescriptor>();
            foreach (var e in dictionary)
                properties.Add(new MyCustomPropertyDescriptor(e.Key, (e.Value?.GetType()) ?? typeof(object)));
            return new PropertyDescriptorCollection(properties.ToArray());
        }
    }
    public class MyCustomPropertyDescriptor : PropertyDescriptor
    {
        Type type;
        string key;
        public MyCustomPropertyDescriptor(string key, Type type)
            : base(key, null)
        {
            this.type = type;
            this.key = key;
        }
        void PropertyChanged(object sender, EventArgs e)
        {
            OnValueChanged(sender, e);
        }
        public override void AddValueChanged(object component, EventHandler handler)
        {
            base.AddValueChanged(component, handler);
            ((INotifyPropertyChanged)component).PropertyChanged += PropertyChanged;
        }
        public override void RemoveValueChanged(object component, EventHandler handler)
        {
            base.RemoveValueChanged(component, handler);
            ((INotifyPropertyChanged)component).PropertyChanged -= PropertyChanged;
        }
        public override Type PropertyType
        {
            get { return type; }
        }
        public override void SetValue(object component, object value)
        {
            ((MyCustomObject)component)[key] = value;
        }
        public override object GetValue(object component)
        {
            return ((MyCustomObject)component)[key];
        }
        public override bool IsReadOnly
        {
            get { return false; }
        }
        public override Type ComponentType
        {
            get { return null; }
        }
        public override bool CanResetValue(object component)
        {
            return false;
        }
        public override void ResetValue(object component)
        {
        }
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
