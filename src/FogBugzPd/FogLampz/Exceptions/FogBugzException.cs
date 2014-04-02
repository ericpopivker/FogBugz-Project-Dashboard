// -----------------------------------------------------------------------
//  <copyright file="FogBugzException.cs"
//             project="FogLampz"
//             assembly="FogLampz"
//             solution="FogLampz"
//             company="Chris Adams Studios">
//      Copyright (c) 1996+. All rights reserved.
//  </copyright>
//  <author id="chris@chrisadams-studios.com">Chris Adams</author>
//  <summary></summary>
// -----------------------------------------------------------------------

#region References

using System;
using System.Runtime.Serialization;

#endregion

namespace FogLampz.Exceptions
{
    /// <summary>
    ///   Exception raised from FogBugz API errors.
    /// </summary>
    [Serializable]
    public class FogBugzException : Exception
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="FogBugzException" /> class.
        /// </summary>
        public FogBugzException()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="FogBugzException" /> class.
        /// </summary>
        /// <param name="message"> The message. </param>
        public FogBugzException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="FogBugzException" /> class.
        /// </summary>
        /// <param name="message"> The message. </param>
        /// <param name="code"> The code. </param>
        public FogBugzException(string message, int code)
            : base(message)
        {
            Code = code;
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="FogBugzException" /> class.
        /// </summary>
        /// <param name="message"> The message. </param>
        /// <param name="inner"> The inner. </param>
        public FogBugzException(string message, Exception inner)
            : base(message, inner)
        {
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="FogBugzException" /> class.
        /// </summary>
        /// <param name="message"> The message. </param>
        /// <param name="code"> The code. </param>
        /// <param name="inner"> The inner. </param>
        public FogBugzException(string message, int code, Exception inner)
            : base(message, inner)
        {
            Code = code;
        }


        /// <summary>
        ///   Initializes a new instance of the <see cref="FogBugzException" /> class.
        /// </summary>
        /// <param name="info"> The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown. </param>
        /// <param name="context"> The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination. </param>
        /// <exception cref="T:System.ArgumentNullException">The
        ///   <paramref name="info" />
        ///   parameter is null.</exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or
        ///   <see cref="P:System.Exception.HResult" />
        ///   is zero (0).</exception>
        protected FogBugzException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        ///   Gets or sets the code.
        /// </summary>
        /// <value> The code. </value>
        public int Code { get; set; }

        /// <summary>
        ///   When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with information about the exception.
        /// </summary>
        /// <param name="info"> The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown. </param>
        /// <param name="context"> The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination. </param>
        /// <exception cref="T:System.ArgumentNullException">The
        ///   <paramref name="info" />
        ///   parameter is a null reference (Nothing in Visual Basic).</exception>
        /// <PermissionSet>
        ///   <IPermission
        ///     class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///     version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
        ///   <IPermission
        ///     class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///     version="1" Flags="SerializationFormatter" />
        /// </PermissionSet>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Code", Code);
        }
    }
}