// -----------------------------------------------------------------------
//  <copyright file="NotLoggedInException.cs"
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
    [Serializable]
    public class NotLoggedInException : Exception
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="NotLoggedInException" /> class.
        /// </summary>
        public NotLoggedInException()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="NotLoggedInException" /> class.
        /// </summary>
        /// <param name="message"> The message. </param>
        public NotLoggedInException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="NotLoggedInException" /> class.
        /// </summary>
        /// <param name="message"> The message. </param>
        /// <param name="inner"> The inner. </param>
        public NotLoggedInException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="NotLoggedInException" /> class.
        /// </summary>
        /// <param name="info"> The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown. </param>
        /// <param name="context"> The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination. </param>
        /// <exception cref="T:System.ArgumentNullException">The
        ///   <paramref name="info" />
        ///   parameter is null.</exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or
        ///   <see cref="P:System.Exception.HResult" />
        ///   is zero (0).</exception>
        protected NotLoggedInException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}