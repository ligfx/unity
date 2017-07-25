﻿using System;
using UnityEngine;

namespace PubNubAPI
{
    public class QueueManager: MonoBehaviour
    {
        private readonly object lockObj = new object();

        public delegate void RunningRequestEndDelegate(PNOperationType operationType);
        public event RunningRequestEndDelegate RunningRequestEnd;
        private bool NoRunningRequests = true;
        internal ushort NoOfConcurrentRequests = 1;
        public PubNubUnity PubNubInstance { get; set;}
        private ushort RunningRequests = 0;

        void Start(){
            this.RunningRequestEnd += delegate(PNOperationType operationType) {
                //Debug.Log(operationType + DateTime.Now.ToLongTimeString());
                UpdateRunningRequests(true);
            };
        }

        void UpdateRunningRequests(bool RequestComplete){
            lock(lockObj){
                if (RequestComplete) {
                    RunningRequests--;
                } else {
                    RunningRequests++;
                }
                //Debug.Log(RunningRequests.ToString() + RequestComplete.ToString());
                if (RunningRequests <= NoOfConcurrentRequests) {
                    NoRunningRequests = true;
                } else {
                    NoRunningRequests = false;
                }
            }
        }

        public void RaiseRunningRequestEnd(PNOperationType operationType){
            this.RunningRequestEnd(operationType);
        }

        void Update(){
            if(PubNubInstance != null){
                if ((RequestQueue.Instance.HasItems) && (NoRunningRequests)) {
                    UpdateRunningRequests(false);
                    QueueStorage qs =  RequestQueue.Instance.Dequeue ();
                    PNOperationType operationType = qs.OperationType;
                    object operationParams = qs.OperationParams;
                    switch(operationType){
                        case PNOperationType.PNTimeOperation:
                            TimeRequestBuilder timebuilder  = operationParams as TimeRequestBuilder;//((TimeBuilder)operationParams);
                            timebuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNWhereNowOperation:
                            WhereNowRequestBuilder whereNowBuilder  = operationParams as WhereNowRequestBuilder;
                            whereNowBuilder.RaiseRunRequest(this);

                            break;
                        case PNOperationType.PNHistoryOperation:
                            HistoryRequestBuilder historyBuilder  = operationParams as HistoryRequestBuilder;
                            historyBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNFireOperation:
                            break;
                        case PNOperationType.PNPublishOperation:
                            PublishRequestBuilder publishBuilder  = operationParams as PublishRequestBuilder;
                            publishBuilder.RaiseRunRequest(this);

                            break;
                        case PNOperationType.PNHereNowOperation:
                            HereNowRequestBuilder hereNowBuilder  = operationParams as HereNowRequestBuilder;
                            hereNowBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNLeaveOperation:
                        //TODO
                            break;
                            
                        case PNOperationType.PNUnsubscribeOperation:
                        //TODO
                            break;
                        case PNOperationType.PNPresenceUnsubscribeOperation:
                        //TODO
                            break;
                        case PNOperationType.PNSetStateOperation:
                            //TODO
                            break;
                        case PNOperationType.PNGetStateOperation:
                            //TODO
                            GetStateRequestBuilder getStateBuilder = operationParams as GetStateRequestBuilder;
                            getStateBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNRemoveAllPushNotificationsOperation:
                            RemoveAllPushChannelsForDeviceRequestBuilder removeAllPushNotificationsRequestBuilder = operationParams as RemoveAllPushChannelsForDeviceRequestBuilder;
                            removeAllPushNotificationsRequestBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNAddPushNotificationsOnChannelsOperation:
                            AddChannelsToPushRequestBuilder addChannelsToGroupBuilder = operationParams as AddChannelsToPushRequestBuilder;
                            addChannelsToGroupBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNPushNotificationEnabledChannelsOperation:
                            ListPushProvisionsRequestBuilder pushNotificationEnabledChannelsRequestBuilder = operationParams as ListPushProvisionsRequestBuilder;
                            pushNotificationEnabledChannelsRequestBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNRemovePushNotificationsFromChannelsOperation:
                            RemoveChannelsFromPushRequestBuilder pushNotificationsFromChannelsRequestBuilder = operationParams as RemoveChannelsFromPushRequestBuilder;
                            pushNotificationsFromChannelsRequestBuilder.RaiseRunRequest(this);
                            break;
                        case PNOperationType.PNAddChannelsToGroupOperation:
                            AddChannelsToChannelGroupRequestBuilder addChannelsToGroupRequestBuilder = operationParams as AddChannelsToChannelGroupRequestBuilder;
                            addChannelsToGroupRequestBuilder.RaiseRunRequest(this);

                            break;
                        case PNOperationType.PNChannelGroupsOperation:
                            GetChannelGroupsRequestBuilder getChannelGroupsBuilder = operationParams as GetChannelGroupsRequestBuilder;
                            getChannelGroupsBuilder.RaiseRunRequest(this);

                            break;
                        case PNOperationType.PNChannelsForGroupOperation:
                            GetAllChannelsForGroupRequestBuilder getChannelsForGroupRequestBuilder = operationParams as GetAllChannelsForGroupRequestBuilder;
                            getChannelsForGroupRequestBuilder.RaiseRunRequest(this);

                            break;
                        case PNOperationType.PNFetchMessagesOperation:
                        //TODO  
                            FetchMessagesRequestBuilder fetchMessagesRequestBuilder = operationParams as FetchMessagesRequestBuilder;
                            fetchMessagesRequestBuilder.RaiseRunRequest(this);

                            break;
                        case PNOperationType.PNRemoveChannelsFromGroupOperation:
                            RemoveChannelsFromGroupRequestBuilder removeChannelsFromGroupRequestBuilder = operationParams as RemoveChannelsFromGroupRequestBuilder;
                            removeChannelsFromGroupRequestBuilder.RaiseRunRequest(this);

                            break;
                        case PNOperationType.PNRemoveGroupOperation:
                            DeleteChannelGroupRequestBuilder removeGroupRequestBuilder = operationParams as DeleteChannelGroupRequestBuilder;
                            removeGroupRequestBuilder.RaiseRunRequest(this);

                            break;
                    }
                }
            } else {
                Debug.Log("PN instance null");
            }
        }
    }
}
