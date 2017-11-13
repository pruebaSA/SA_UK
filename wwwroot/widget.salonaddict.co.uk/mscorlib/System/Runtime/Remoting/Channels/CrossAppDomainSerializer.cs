namespace System.Runtime.Remoting.Channels
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    internal static class CrossAppDomainSerializer
    {
        internal static IMessage DeserializeMessage(MemoryStream stm) => 
            DeserializeMessage(stm, null);

        internal static IMessage DeserializeMessage(MemoryStream stm, IMethodCallMessage reqMsg)
        {
            if (stm == null)
            {
                throw new ArgumentNullException("stm");
            }
            stm.Position = 0L;
            BinaryFormatter formatter = new BinaryFormatter {
                SurrogateSelector = null,
                Context = new StreamingContext(StreamingContextStates.CrossAppDomain)
            };
            return (IMessage) formatter.Deserialize(stm, null, false, true, reqMsg);
        }

        internal static ArrayList DeserializeMessageParts(MemoryStream stm) => 
            ((ArrayList) DeserializeObject(stm));

        internal static object DeserializeObject(MemoryStream stm)
        {
            stm.Position = 0L;
            BinaryFormatter formatter = new BinaryFormatter {
                Context = new StreamingContext(StreamingContextStates.CrossAppDomain)
            };
            return formatter.Deserialize(stm, null, false, true, null);
        }

        internal static MemoryStream SerializeMessage(IMessage msg)
        {
            MemoryStream serializationStream = new MemoryStream();
            RemotingSurrogateSelector selector = new RemotingSurrogateSelector();
            new BinaryFormatter { 
                SurrogateSelector = selector,
                Context = new StreamingContext(StreamingContextStates.CrossAppDomain)
            }.Serialize(serializationStream, msg, null, false);
            serializationStream.Position = 0L;
            return serializationStream;
        }

        internal static MemoryStream SerializeMessageParts(ArrayList argsToSerialize)
        {
            MemoryStream serializationStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            RemotingSurrogateSelector selector = new RemotingSurrogateSelector();
            formatter.SurrogateSelector = selector;
            formatter.Context = new StreamingContext(StreamingContextStates.CrossAppDomain);
            formatter.Serialize(serializationStream, argsToSerialize, null, false);
            serializationStream.Position = 0L;
            return serializationStream;
        }

        internal static MemoryStream SerializeObject(object obj)
        {
            MemoryStream stm = new MemoryStream();
            SerializeObject(obj, stm);
            stm.Position = 0L;
            return stm;
        }

        internal static void SerializeObject(object obj, MemoryStream stm)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            RemotingSurrogateSelector selector = new RemotingSurrogateSelector();
            formatter.SurrogateSelector = selector;
            formatter.Context = new StreamingContext(StreamingContextStates.CrossAppDomain);
            formatter.Serialize(stm, obj, null, false);
        }
    }
}

