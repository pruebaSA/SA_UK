﻿namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Security;
    using System.Xml;

    internal class Wsrm11Index : WsrmIndex
    {
        private ActionHeader ackRequestedActionHeader;
        private AddressingVersion addressingVersion;
        private ActionHeader closeSequenceActionHeader;
        private ActionHeader closeSequenceResponseActionHeader;
        private ActionHeader createSequenceActionHeader;
        private ActionHeader sequenceAcknowledgementActionHeader;
        private static MessagePartSpecification signedReliabilityMessageParts;
        private ActionHeader terminateSequenceActionHeader;
        private ActionHeader terminateSequenceResponseActionHeader;

        internal Wsrm11Index(AddressingVersion addressingVersion)
        {
            this.addressingVersion = addressingVersion;
        }

        protected override ActionHeader GetActionHeader(string element)
        {
            Wsrm11Dictionary dictionary = DXD.Wsrm11Dictionary;
            if (element == "AckRequested")
            {
                if (this.ackRequestedActionHeader == null)
                {
                    this.ackRequestedActionHeader = ActionHeader.Create(dictionary.AckRequestedAction, this.addressingVersion);
                }
                return this.ackRequestedActionHeader;
            }
            if (element == "CreateSequence")
            {
                if (this.createSequenceActionHeader == null)
                {
                    this.createSequenceActionHeader = ActionHeader.Create(dictionary.CreateSequenceAction, this.addressingVersion);
                }
                return this.createSequenceActionHeader;
            }
            if (element == "SequenceAcknowledgement")
            {
                if (this.sequenceAcknowledgementActionHeader == null)
                {
                    this.sequenceAcknowledgementActionHeader = ActionHeader.Create(dictionary.SequenceAcknowledgementAction, this.addressingVersion);
                }
                return this.sequenceAcknowledgementActionHeader;
            }
            if (element == "TerminateSequence")
            {
                if (this.terminateSequenceActionHeader == null)
                {
                    this.terminateSequenceActionHeader = ActionHeader.Create(dictionary.TerminateSequenceAction, this.addressingVersion);
                }
                return this.terminateSequenceActionHeader;
            }
            if (element == "TerminateSequenceResponse")
            {
                if (this.terminateSequenceResponseActionHeader == null)
                {
                    this.terminateSequenceResponseActionHeader = ActionHeader.Create(dictionary.TerminateSequenceResponseAction, this.addressingVersion);
                }
                return this.terminateSequenceResponseActionHeader;
            }
            if (element == "CloseSequence")
            {
                if (this.closeSequenceActionHeader == null)
                {
                    this.closeSequenceActionHeader = ActionHeader.Create(dictionary.CloseSequenceAction, this.addressingVersion);
                }
                return this.closeSequenceActionHeader;
            }
            if (element != "CloseSequenceResponse")
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
            if (this.closeSequenceResponseActionHeader == null)
            {
                this.closeSequenceResponseActionHeader = ActionHeader.Create(dictionary.CloseSequenceResponseAction, this.addressingVersion);
            }
            return this.closeSequenceResponseActionHeader;
        }

        internal static MessagePartSpecification SignedReliabilityMessageParts
        {
            get
            {
                if (signedReliabilityMessageParts == null)
                {
                    XmlQualifiedName[] headerTypes = new XmlQualifiedName[] { new XmlQualifiedName("Sequence", "http://docs.oasis-open.org/ws-rx/wsrm/200702"), new XmlQualifiedName("SequenceAcknowledgement", "http://docs.oasis-open.org/ws-rx/wsrm/200702"), new XmlQualifiedName("AckRequested", "http://docs.oasis-open.org/ws-rx/wsrm/200702"), new XmlQualifiedName("UsesSequenceSTR", "http://docs.oasis-open.org/ws-rx/wsrm/200702") };
                    MessagePartSpecification specification = new MessagePartSpecification(headerTypes);
                    specification.MakeReadOnly();
                    signedReliabilityMessageParts = specification;
                }
                return signedReliabilityMessageParts;
            }
        }
    }
}

