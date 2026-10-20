import { environment } from './../../environments/environment';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

/** ISO 8601 durations (e.g. PT1H, P14D) are used for every time span. */
export interface TopicSettings {
    defaultMessageTimeToLive?: string | null;
    duplicateDetectionHistoryTimeWindow?: string | null;
    autoDeleteOnIdle?: string | null;
    requiresDuplicateDetection?: boolean | null;
    maxSizeInMegabytes?: number | null;
}

export interface SubscriptionSettings {
    lockDuration?: string | null;
    defaultMessageTimeToLive?: string | null;
    autoDeleteOnIdle?: string | null;
    maxDeliveryCount?: number | null;
    deadLetteringOnMessageExpiration?: boolean | null;
    requiresSession?: boolean | null;
    forwardTo?: string | null;
    forwardDeadLetteredMessagesTo?: string | null;
}

export interface QueueSettings extends SubscriptionSettings {
    duplicateDetectionHistoryTimeWindow?: string | null;
    requiresDuplicateDetection?: boolean | null;
    maxSizeInMegabytes?: number | null;
}

export type RuleFilterType = 'Sql' | 'Correlation' | 'True' | 'False';

export interface RuleSettings {
    filterType?: RuleFilterType | null;
    sqlExpression?: string | null;
    actionExpression?: string | null;
    correlationId?: string | null;
    messageId?: string | null;
    to?: string | null;
    replyTo?: string | null;
    subject?: string | null;
    sessionId?: string | null;
    replyToSessionId?: string | null;
    contentType?: string | null;
    applicationProperties?: Record<string, string> | null;
}

export interface RuleInfo {
    name: string;
    properties: RuleSettings;
}

export interface Subscription {
    name: string;
    properties: SubscriptionSettings;
    rules: RuleInfo[];
}

export interface Topic {
    topicName: string;
    properties: TopicSettings;
    subscriptions: Subscription[];
}

export interface Queue {
    name: string;
    properties: QueueSettings;
}

@Injectable({ providedIn: 'root' })
export class EntitiesService {
    private readonly entitiesEndpoint = `${environment.apiBaseUrl}/api/Entities`;

    constructor(private http: HttpClient) {}

    /**
     * GET /api/Entities
     * Retorna a lista de tópicos, suas configurações e subscriptions.
     */
    getEntities(): Observable<Topic[]> {
        return this.http.get<Topic[]>(this.entitiesEndpoint);
    }

    /** GET /api/Entities/queues */
    getQueues(): Observable<Queue[]> {
        return this.http.get<Queue[]>(`${this.entitiesEndpoint}/queues`);
    }

    createTopic(name: string, settings: TopicSettings): Observable<Topic> {
        return this.http.post<Topic>(this.topicUrl(name), settings);
    }

    updateTopic(name: string, settings: TopicSettings): Observable<void> {
        return this.http.put<void>(this.topicUrl(name), settings);
    }

    deleteTopic(name: string): Observable<void> {
        return this.http.delete<void>(this.topicUrl(name));
    }

    createSubscription(topic: string, name: string, settings: SubscriptionSettings): Observable<Subscription> {
        return this.http.post<Subscription>(this.subscriptionUrl(topic, name), settings);
    }

    updateSubscription(topic: string, name: string, settings: SubscriptionSettings): Observable<void> {
        return this.http.put<void>(this.subscriptionUrl(topic, name), settings);
    }

    deleteSubscription(topic: string, name: string): Observable<void> {
        return this.http.delete<void>(this.subscriptionUrl(topic, name));
    }

    createRule(topic: string, subscription: string, name: string, settings: RuleSettings): Observable<RuleInfo> {
        return this.http.post<RuleInfo>(this.ruleUrl(topic, subscription, name), settings);
    }

    updateRule(topic: string, subscription: string, name: string, settings: RuleSettings): Observable<void> {
        return this.http.put<void>(this.ruleUrl(topic, subscription, name), settings);
    }

    deleteRule(topic: string, subscription: string, name: string): Observable<void> {
        return this.http.delete<void>(this.ruleUrl(topic, subscription, name));
    }

    createQueue(name: string, settings: QueueSettings): Observable<Queue> {
        return this.http.post<Queue>(this.queueUrl(name), settings);
    }

    updateQueue(name: string, settings: QueueSettings): Observable<void> {
        return this.http.put<void>(this.queueUrl(name), settings);
    }

    deleteQueue(name: string): Observable<void> {
        return this.http.delete<void>(this.queueUrl(name));
    }

    private topicUrl(name: string): string {
        return `${this.entitiesEndpoint}/topics/${encodeURIComponent(name)}`;
    }

    private subscriptionUrl(topic: string, name: string): string {
        return `${this.topicUrl(topic)}/subscriptions/${encodeURIComponent(name)}`;
    }

    private ruleUrl(topic: string, subscription: string, name: string): string {
        return `${this.subscriptionUrl(topic, subscription)}/rules/${encodeURIComponent(name)}`;
    }

    private queueUrl(name: string): string {
        return `${this.entitiesEndpoint}/queues/${encodeURIComponent(name)}`;
    }
}
