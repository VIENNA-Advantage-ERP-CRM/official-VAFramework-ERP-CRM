/********************************************************
 * Module Name    :     Report
 * Purpose        :     Excel Report Preview
 * Author         :     Jagmohan Bhatt
 * Date           :     28-Jul-2009
  ******************************************************/
namespace VAdvantage.Print
{
	using System;
	using System.ComponentModel;

	/// <summary>
	/// Represents a row from array view.
	/// </summary>
	public class ArrayRowView : ICustomTypeDescriptor, IEditableObject, IDataErrorInfo
	{
		/// <summary>
		/// Owner of the row.
		/// </summary>
		private ArrayDataView	_owner;
		private int				_index;

		string					_error;

		internal ArrayRowView(ArrayDataView owner,int index)
		{
			_owner = owner;
			_index = index;
		}

		

		internal object GetColumn(int index)
		{
			return _owner._data.GetValue(_index,index);
		}

		internal void SetColumnValue(int index,object value)
		{
			try
			{
				_owner._data.SetValue(value,_index,index);
			}
			catch(Exception e)
			{
				_error = e.ToString();
			}
		}

		#region ICustomTypeDescriptor Members

		public TypeConverter GetConverter()
		{
			// TODO:  Add ArrayColumn.GetConverter implementation
			return null;
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return EventDescriptorCollection.Empty;
		}

		EventDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetEvents()
		{
			return EventDescriptorCollection.Empty;
		}

		public string GetComponentName()
		{
			return null;
		}

		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return _owner;
		}

		public AttributeCollection GetAttributes()
		{
			return AttributeCollection.Empty;
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			int col = _owner._data.GetLength(1);
			Type type = _owner._data.GetType().GetElementType();
			PropertyDescriptor[] prop = new PropertyDescriptor[col];
			for(int i = 0; i < col; i++)
			{
				prop[i] = new ArrayPropertyDescriptor(_owner.ColumnNames[i],type,i);
			}
			return new PropertyDescriptorCollection(prop);
		}

		PropertyDescriptorCollection System.ComponentModel.ICustomTypeDescriptor.GetProperties()
		{
			return GetProperties(null);
		}

		public object GetEditor(Type editorBaseType)
		{
			return null;
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			return null;
		}

		public EventDescriptor GetDefaultEvent()
		{
			return null;
		}

		public string GetClassName()
		{
			return this.GetType().Name;
		}

		#endregion

		#region IEditableObject Members

		public void EndEdit()
		{
			// TODO:  Add ArrayRowView.EndEdit implementation
		}

		public void CancelEdit()
		{
			// TODO:  Add ArrayRowView.CancelEdit implementation
		}

		public void BeginEdit()
		{
			// TODO:  Add ArrayRowView.BeginEdit implementation
		}

		#endregion

		#region IDataErrorInfo Members

		public string this[string columnName]
		{
			get
			{
				// TODO:  Add ArrayRowView.this getter implementation
				return null;
			}
		}

		public string Error
		{
			get
			{
				// TODO:  Add ArrayRowView.Error getter implementation
				return null;
			}
		}

		#endregion
	}// END CLASS DEFINITION ArrayRowView

} 
