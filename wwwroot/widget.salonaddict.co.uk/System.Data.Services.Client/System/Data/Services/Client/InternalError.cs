namespace System.Data.Services.Client
{
    using System;

    internal enum InternalError
    {
        ChangeResponseMissingContentID = 15,
        ChangeResponseUnknownContentID = 0x10,
        EntityNotAddedState = 9,
        EntityNotDeleted = 8,
        EntryNotModified = 11,
        InvalidAsyncResponseStreamCopy = 0x2c,
        InvalidAsyncResponseStreamCopyBuffer = 0x2d,
        InvalidBeginNextChange = 0x3d,
        InvalidEndGetRequestCompleted = 0x15,
        InvalidEndGetRequestStream = 20,
        InvalidEndGetRequestStreamContent = 0x18,
        InvalidEndGetRequestStreamContentLength = 0x19,
        InvalidEndGetRequestStreamRequest = 0x16,
        InvalidEndGetRequestStreamStream = 0x17,
        InvalidEndGetResponse = 40,
        InvalidEndGetResponseCompleted = 0x29,
        InvalidEndGetResponseRequest = 0x2a,
        InvalidEndGetResponseResponse = 0x2b,
        InvalidEndRead = 50,
        InvalidEndReadBuffer = 0x36,
        InvalidEndReadCompleted = 0x33,
        InvalidEndReadCopy = 0x35,
        InvalidEndReadStream = 0x34,
        InvalidEndWrite = 30,
        InvalidEndWriteCompleted = 0x1f,
        InvalidEndWriteRequest = 0x20,
        InvalidEndWriteStream = 0x21,
        InvalidGetRequestStream = 70,
        InvalidGetResponse = 0x47,
        InvalidSaveNextChange = 60,
        LinkBadState = 12,
        LinkNotAddedState = 10,
        NullResponseStream = 7,
        SaveNextChangeIncomplete = 0x3e,
        TooManyBatchResponse = 0x11,
        UnexpectedBatchState = 14,
        UnexpectedBeginChangeSet = 13,
        UnexpectedEndWhenSkipping = 3,
        UnexpectedReadState = 4,
        UnexpectedRequestBufferSizeTooSmall = 5,
        UnexpectedXmlNodeTypeWhenReading = 1,
        UnexpectedXmlNodeTypeWhenSkipping = 2,
        UnvalidatedEntityState = 6
    }
}

