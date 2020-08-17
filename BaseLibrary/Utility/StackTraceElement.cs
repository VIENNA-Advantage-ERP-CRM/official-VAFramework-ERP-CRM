/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : StackTraceElement
 * Purpose        : 
 * Class Used     : 
 * Chronological    Development
 * Raghunandan      28-May-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Collections;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.SqlExec;
using System.Runtime.CompilerServices;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace VAdvantage.Utility
{
    [Serializable]
    public class StackTraceElement //: ISerializable
    {
        #region Private variables
        // Normally initialized by VM (public constructor added in 1.5)
        private String declaringClass;
        private String methodName;
        private String fileName;
        private int lineNumber;
       // private static long serialVersionUID = 6992337162326171013L;
        #endregion

        /**
         * Creates a stack trace element representing the specified execution
         * point.
         *
         * @param declaringClass the fully qualified name of the class containing
         *        the execution point represented by the stack trace element
         * @param methodName the name of the method containing the execution point
         *        represented by the stack trace element
         * @param fileName the name of the file containing the execution point
         *         represented by the stack trace element, or <tt>null</tt> if
         *         this information is unavailable
         * @param lineNumber the line number of the source line containing the
         *         execution point represented by this stack trace element, or
         *         a negative number if this information is unavailable. A value
         *         of -2 indicates that the method containing the execution point
         *         is a native method
         * @throws NullPointerException if <tt>declaringClass</tt> or
         *         <tt>methodName</tt> is null
         * @since 1.5
         */
        public StackTraceElement(String declaringClass, String methodName, string fileName, int lineNumber)
        {
            if (declaringClass == null)
            {
                throw new Exception("Declaring class is null");
            }
            if (methodName == null)
            {
                throw new Exception("Method name is null");
            }

            this.declaringClass = declaringClass;
            this.methodName = methodName;
            this.fileName = fileName;
            this.lineNumber = lineNumber;
        }

        /**
         * Returns the name of the source file containing the execution point
         * represented by this stack trace element.  Generally, this corresponds
         * to the <tt>SourceFile</tt> attribute of the relevant <tt>class</tt>
         * file (as per <i>The Java Virtual Machine Specification</i>, Section
         * 4.7.7).  In some systems, the name may refer to some source code unit
         * other than a file, such as an entry in source repository.
         *
         * @return the name of the file containing the execution point
         *         represented by this stack trace element, or <tt>null</tt> if
         *         this information is unavailable.
         */
        public String GetFileName()
        {
            return fileName;
        }

        /**
         * Returns the line number of the source line containing the execution
         * point represented by this stack trace element.  Generally, this is
         * derived from the <tt>LineNumberTable</tt> attribute of the relevant
         * <tt>class</tt> file (as per <i>The Java Virtual Machine
         * Specification</i>, Section 4.7.8).
         *
         * @return the line number of the source line containing the execution
         *         point represented by this stack trace element, or a negative
         *         number if this information is unavailable.
         */
        public int GetLineNumber()
        {
            return lineNumber;
        }

        /**
         * Returns the fully qualified name of the class containing the
         * execution point represented by this stack trace element.
         *
         * @return the fully qualified name of the <tt>Class</tt> containing
         *         the execution point represented by this stack trace element.
         */
        public String GetClassName()
        {
            return declaringClass;
        }

        /**
         * Returns the name of the method containing the execution point
         * represented by this stack trace element.  If the execution point is
         * contained in an instance or class initializer, this method will return
         * the appropriate <i>special method name</i>, <tt>&lt;init&gt;</tt> or
         * <tt>&lt;clinit&gt;</tt>, as per Section 3.9 of <i>The Java Virtual
         * Machine Specification</i>.
         *
         * @return the name of the method containing the execution point
         *         represented by this stack trace element.
         */
        public String GetMethodName()
        {
            return methodName;
        }

        /**
         * Returns true if the method containing the execution point
         * represented by this stack trace element is a native method.
         *
         * @return <tt>true</tt> if the method containing the execution point
         *         represented by this stack trace element is a native method.
         */
        public bool IsNativeMethod()
        {
            return lineNumber == -2;
        }

        /**
         * Returns a string representation of this stack trace element.  The
         * format of this string depends on the implementation, but the following
         * examples may be regarded as typical:
         * <ul>
         * <li>
         *   <tt>"MyClass.mash(MyClass.java:9)"</tt> - Here, <tt>"MyClass"</tt>
         *   is the <i>fully-qualified name</i> of the class containing the
         *   execution point represented by this stack trace element,
         *   <tt>"mash"</tt> is the name of the method containing the execution
         *   point, <tt>"MyClass.java"</tt> is the source file containing the
         *   execution point, and <tt>"9"</tt> is the line number of the source
         *   line containing the execution point.
         * <li>
         *   <tt>"MyClass.mash(MyClass.java)"</tt> - As above, but the line
         *   number is unavailable.
         * <li>
         *   <tt>"MyClass.mash(Unknown Source)"</tt> - As above, but neither
         *   the file name nor the line  number are available.
         * <li>
         *   <tt>"MyClass.mash(Native Method)"</tt> - As above, but neither
         *   the file name nor the line  number are available, and the method
         *   containing the execution point is known to be a native method.
         * </ul>
         * @see    Throwable#printStackTrace()
         */
        public override String ToString()
        {
            return GetClassName() + "." + methodName +
                (IsNativeMethod() ? "(Native Method)" :
                 (fileName != null && lineNumber >= 0 ?
                  "(" + fileName + ":" + lineNumber + ")" :
                  (fileName != null ? "(" + fileName + ")" : "(Unknown Source)")));
        }

        /**
         * Returns true if the specified object is another
         * <tt>StackTraceElement</tt> instance representing the same execution
         * point as this instance.  Two stack trace elements <tt>a</tt> and
         * <tt>b</tt> are equal if and only if:
         * <pre>
         *     equals(a.getFileName(), b.getFileName()) &&
         *     a.getLineNumber() == b.getLineNumber()) &&
         *     equals(a.getClassName(), b.getClassName()) &&
         *     equals(a.getMethodName(), b.getMethodName())
         * </pre>
         * where <tt>equals</tt> is defined as:
         * <pre>
         *     static boolean equals(Object a, Object b) {
         *         return a==b || (a != null && a.equals(b));
         *     }
         * </pre>
         * 
         * @param  obj the object to be compared with this stack trace element.
         * @return true if the specified object is another
         *         <tt>StackTraceElement</tt> instance representing the same
         *         execution point as this instance.
         */
        public override bool Equals(Object obj)
        {
            if (obj == this)
                return true;
            if (!(obj is StackTraceElement))
            {
                return false;
            }
            StackTraceElement e = (StackTraceElement)obj;
            return e.declaringClass.Equals(declaringClass) && e.lineNumber == lineNumber
                && Eq(methodName, e.methodName) && Eq(fileName, e.fileName);
        }

        private static bool Eq(Object a, Object b)
        {
            return a == b || (a != null && a.Equals(b));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns a hash code value for this stack trace element.
        /// </summary>
        /// <returns></returns>
        public int HashCode()
        {
            //int result = 31*declaringClass.hashCode() + methodName.hashCode();
            int result = int.Parse(31 * int.Parse(declaringClass) + methodName);
            //result = 31 * result + (fileName == null ? 0 : fileName.hashCode());
            result = 31 * result + (fileName == null ? 0 : fileName.GetHashCode());
            result = 31 * result + lineNumber;
            return result;
        }
    }


    
}
