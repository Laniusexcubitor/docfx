﻿namespace Microsoft.DocAsCode.Plugins
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable]
    public class DocumentException : Exception
    {
        public string File { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public string Code { get; set; }

        public DocumentException() { }
        public DocumentException(string message) : base(message) { }
        public DocumentException(string message, string code) : this(message)
        {
            Code = code;
        }
        public DocumentException(string message, Exception inner) : base(message, inner) { }
        public DocumentException(string message, string code, Exception inner) : base(message, inner)
        {
            Code = code;
        }
        protected DocumentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Line = info.GetInt32(nameof(Line));
            Column = info.GetInt32(nameof(Column));
            File = info.GetString(nameof(File));
            Code = info.GetString(nameof(Code));
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info,
            StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Line), Line);
            info.AddValue(nameof(Column), Column);
            info.AddValue(nameof(File), File);
            info.AddValue(nameof(Code), Code);
        }

        public static void RunAll(params Action[] actions)
        {
            if (actions == null)
            {
                throw new ArgumentNullException(nameof(actions));
            }
            DocumentException firstException = null;
            foreach (var action in actions)
            {
                try
                {
                    action();
                }
                catch (DocumentException ex)
                {
                    if (firstException == null)
                    {
                        firstException = ex;
                    }
                }
            }
            if (firstException != null)
            {
                throw new DocumentException(firstException.Message, firstException);
            }
        }
    }
}
