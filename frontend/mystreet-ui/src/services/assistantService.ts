import api from './api';
import type { ChatAssistantRequest, ChatAssistantResponse } from '../types/assistant';

export const assistantService = {
  chat: async (payload: ChatAssistantRequest) => {
    const res = await api.post<ChatAssistantResponse>('/assistant/chat', payload);
    return res.data;
  }
};
