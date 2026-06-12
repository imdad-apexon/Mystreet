import { useMemo, useState } from 'react';
import { Link } from 'react-router-dom';
import { assistantService } from '../services/assistantService';
import type { Product } from '../types/product';

type AssistantRole = 'user' | 'assistant';

type AssistantMessage = {
  id: string;
  role: AssistantRole;
  text: string;
  recommendations?: Product[];
};

const starterMessage: AssistantMessage = {
  id: 'starter',
  role: 'assistant',
  text: 'Hi! I can help with product recommendations, shipping policy, and return policy. Ask me anything about shopping.'
};

export default function ShoppingAssistant() {
  const [isOpen, setIsOpen] = useState(false);
  const [input, setInput] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [messages, setMessages] = useState<AssistantMessage[]>([starterMessage]);

  const closeAssistant = () => {
    setIsOpen(false);
    setInput('');
    setIsLoading(false);
    setMessages([starterMessage]);
  };

  const canSend = useMemo(() => input.trim().length > 0 && !isLoading, [input, isLoading]);

  const handleSend = async () => {
    const message = input.trim();
    if (!message || isLoading) return;

    const userMessage: AssistantMessage = {
      id: `user-${Date.now()}`,
      role: 'user',
      text: message
    };

    setMessages((prev) => [...prev, userMessage]);
    setInput('');
    setIsLoading(true);

    try {
      const res = await assistantService.chat({ message, productLimit: 4 });
      const assistantMessage: AssistantMessage = {
        id: `assistant-${Date.now()}`,
        role: 'assistant',
        text: res.reply,
        recommendations: res.recommendedProducts
      };
      setMessages((prev) => [...prev, assistantMessage]);
    } catch {
      setMessages((prev) => [
        ...prev,
        {
          id: `assistant-error-${Date.now()}`,
          role: 'assistant',
          text: 'Assistant is temporarily unavailable. Please try again in a moment.'
        }
      ]);
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="assistant-shell">
      {isOpen && (
        <section className="assistant-panel" aria-label="AI Shopping Assistant">
          <header className="assistant-panel__header">
            <div>
              <h3>AI Shopping Assistant</h3>
              <p>Ask products, shipping, returns, recommendations</p>
            </div>
            <button type="button" className="assistant-panel__close" onClick={closeAssistant}>
              Close
            </button>
          </header>

          <div className="assistant-messages">
            {messages.map((msg) => (
              <article
                key={msg.id}
                className={msg.role === 'user' ? 'assistant-msg assistant-msg--user' : 'assistant-msg assistant-msg--bot'}
              >
                <p>{msg.text}</p>
                {msg.recommendations && msg.recommendations.length > 0 && (
                  <div className="assistant-recos">
                    {msg.recommendations.map((item) => (
                      <Link key={item.id} className="assistant-reco" to={`/products/${item.id}`} onClick={() => setIsOpen(false)}>
                        <span className="assistant-reco__name">{item.name}</span>
                        <span className="assistant-reco__meta">{item.brand} • ${item.price.toFixed(2)}</span>
                      </Link>
                    ))}
                  </div>
                )}
              </article>
            ))}
            {isLoading && (
              <article className="assistant-msg assistant-msg--bot">
                <p>Thinking...</p>
              </article>
            )}
          </div>

          <footer className="assistant-input-wrap">
            <input
              value={input}
              onChange={(e) => setInput(e.target.value)}
              placeholder="Example: Which laptop is best for programming?"
              onKeyDown={(e) => {
                if (e.key === 'Enter') {
                  e.preventDefault();
                  void handleSend();
                }
              }}
            />
            <button type="button" onClick={() => void handleSend()} disabled={!canSend}>
              Send
            </button>
          </footer>
        </section>
      )}

      <button
        type="button"
        className="assistant-fab"
        onClick={() => {
          if (isOpen) {
            closeAssistant();
            return;
          }
          setIsOpen(true);
        }}
      >
        {isOpen ? 'Hide Assistant' : 'Ask AI'}
      </button>
    </div>
  );
}
