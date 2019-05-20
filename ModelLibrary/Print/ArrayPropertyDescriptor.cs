/********************************************************
 * Module Name    :     Report
 * Purpose        :     Excel Report Preview
 * Author         :     Jagmohan Bhatt
 * Date           :     28-Jul-2009
  ******************************************************/

namespace VAdvantage.Print
{

	using System;
	using System.Diagnostics;
	using System.ComponentModel;
	/// <summary>
	/// Property decriptor for array
	/// </summary>
	public class ArrayPropertyDescriptor	: PropertyDescriptor
	{
		private string		_name;
		private Type		_type;
		private int			_index;

		public ArrayPropertyDescriptor(string name,Type type,int index) : base(name==""?"NA":name,null)
		{
			_name	= name;
			_type	= type;
			_index  = index;
		}

		public override string DisplayName
		{
			get
			{
				return _name;
			}
		}

		public override Type ComponentType
		{
			get
			{
				return typeof(ArrayRowView);
			}
		}
		
		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}
		public override Type PropertyType
		{
			get
			{
				return _type;
			}
		}
		
		public override object GetValue(object component)
		{
			try
			{

				return ((ArrayRowView)component).GetColumn(_index);
			}
			catch(Exception e)
			{
				Debug.WriteLine(e);
			}

			Debug.Assert(false);

			return null;
		}

		public override void SetValue(object component, object value)
		{
			try
			{
				((ArrayRowView)component).SetColumnValue(_index,value);
			}
			catch(Exception e)
			{
				Debug.WriteLine(e);
				Debug.Assert(false);
			}

			
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
	}// END CLASS DEFINITION ArrayPropertyDescriptor

} 
