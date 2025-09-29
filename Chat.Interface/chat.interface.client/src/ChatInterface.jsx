import React, { useState, useEffect, useRef } from 'react';
import { Send, Bot, User, Sparkles } from 'lucide-react';
import axios from 'axios';

const ChatInterface = ({ connection, dataSources, onCampaignGenerated }) => {
  const [messages, setMessages] = useState([
    {
      id: 1,
      content: "Hi! I'm your campaign intelligence assistant. I can help you create targeted campaigns based on your data sources. What would you like to achieve today?",
      type: 'assistant',
      timestamp: new Date()
    }
  ]);
  const [inputMessage, setInputMessage] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [selectedChannels, setSelectedChannels] = useState([]);
  const [availableChannels, setAvailableChannels] = useState([]);
  const messagesEndRef = useRef(null);

  useEffect(() => {
    fetchChannels();
  }, []);

  useEffect(() => {
    if (connection) {
      connection.on('ReceiveMessage', (message) => {
        setMessages(prev => [...prev, {
          ...message,
          timestamp: new Date(message.timestamp)
        }]);
        setIsLoading(false);
      });
    }
  }, [connection]);

  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [messages]);

  const fetchChannels = async () => {
    try {
      const response = await axios.get('/api/campaign/channels');
      setAvailableChannels(response.data);
      setSelectedChannels(response.data.slice(0, 2).map(c => c.value)); // Select first 2 by default
    } catch (error) {
      console.error('Error fetching channels:', error);
    }
  };

  const handleSendMessage = async () => {
    if (!inputMessage.trim()) return;

    const userMessage = {
      id: Date.now(),
      content: inputMessage,
      type: 'user',
      timestamp: new Date()
    };

    setMessages(prev => [...prev, userMessage]);
    setIsLoading(true);

    try {
      // Check if this looks like a campaign request
      if (inputMessage.toLowerCase().includes('campaign') || 
          inputMessage.toLowerCase().includes('promote') ||
          inputMessage.toLowerCase().includes('advertise')) {
        
        await generateCampaign(inputMessage);
      } else {
        // Regular chat message
        await axios.post('/api/chat/send', {
          message: inputMessage,
          sessionId: null
        });
      }
    } catch (error) {
      console.error('Error sending message:', error);
      setIsLoading(false);
    }

    setInputMessage('');
  };

  const generateCampaign = async (prompt) => {
    try {
      const connectedSources = dataSources.filter(ds => ds.isConnected);
      const response = await axios.post('/api/campaign/generate', {
        prompt,
        dataSourceIds: connectedSources.map(ds => ds.id),
        preferredChannels: selectedChannels
      });

      const campaign = response.data;
      
      // Add campaign message to chat
      const campaignMessage = {
        id: Date.now() + 1,
        content: `🎯 **Campaign Generated Successfully!**\n\n**${campaign.name}**\n\n📧 **Message:** ${campaign.message}\n\n📊 **Audience:** ${campaign.audience.name} (Est. ${campaign.audience.estimatedSize.toLocaleString()} people)\n\n📅 **Optimal Time:** ${new Date(campaign.timing.optimalTime).toLocaleString()}\n\n🚀 **Channels:** ${campaign.channels.join(', ')}\n\nReady to launch when you are!`,
        type: 'assistant',
        timestamp: new Date(),
        campaign: campaign
      };

      setMessages(prev => [...prev, campaignMessage]);
      onCampaignGenerated(campaign);
      setIsLoading(false);
    } catch (error) {
      console.error('Error generating campaign:', error);
      setIsLoading(false);
    }
  };

  const handleChannelToggle = (channelValue) => {
    setSelectedChannels(prev => 
      prev.includes(channelValue)
        ? prev.filter(c => c !== channelValue)
        : [...prev, channelValue]
    );
  };

  return (
    <div className="grid grid-cols-1 lg:grid-cols-4 gap-8 h-[calc(100vh-200px)]">
      {/* Chat Area */}
      <div className="lg:col-span-3 bg-white rounded-lg border border-gray-200 flex flex-col">
        {/* Messages */}
        <div className="flex-1 p-6 overflow-y-auto space-y-4">
          {messages.map(message => (
            <div
              key={message.id}
              className={`flex ${message.type === 'user' ? 'justify-end' : 'justify-start'}`}
            >
              <div className={`chat-message ${message.type}`}>
                <div className="flex items-start space-x-2">
                  {message.type === 'assistant' && (
                    <div className="flex-shrink-0 w-6 h-6 bg-primary-500 rounded-full flex items-center justify-center">
                      <Bot size={14} className="text-white" />
                    </div>
                  )}
                  {message.type === 'user' && (
                    <div className="flex-shrink-0 w-6 h-6 bg-gray-500 rounded-full flex items-center justify-center">
                      <User size={14} className="text-white" />
                    </div>
                  )}
                  <div className="flex-1">
                    <div className="whitespace-pre-wrap">{message.content}</div>
                    <div className="text-xs opacity-70 mt-1">
                      {message.timestamp.toLocaleTimeString()}
                    </div>
                  </div>
                </div>
              </div>
            </div>
          ))}
          {isLoading && (
            <div className="flex justify-start">
              <div className="chat-message assistant">
                <div className="flex items-center space-x-2">
                  <div className="animate-spin">
                    <Sparkles size={16} />
                  </div>
                  <span>Generating campaign...</span>
                </div>
              </div>
            </div>
          )}
          <div ref={messagesEndRef} />
        </div>

        {/* Input Area */}
        <div className="border-t border-gray-200 p-4">
          <div className="flex space-x-4">
            <input
              type="text"
              value={inputMessage}
              onChange={(e) => setInputMessage(e.target.value)}
              onKeyPress={(e) => e.key === 'Enter' && handleSendMessage()}
              placeholder="Ask about campaign strategies, audience targeting, or data insights..."
              className="flex-1 border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:border-primary-500"
              disabled={isLoading}
            />
            <button
              onClick={handleSendMessage}
              disabled={isLoading || !inputMessage.trim()}
              className="bg-primary-500 text-white p-2 rounded-lg hover:bg-primary-600 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              <Send size={20} />
            </button>
          </div>
        </div>
      </div>

      {/* Sidebar */}
      <div className="space-y-6">
        {/* Channel Selection */}
        <div className="bg-white p-4 rounded-lg border border-gray-200">
          <h3 className="font-semibold text-gray-900 mb-3">Preferred Channels</h3>
          <div className="space-y-2">
            {availableChannels.map(channel => (
              <label key={channel.value} className="flex items-center">
                <input
                  type="checkbox"
                  checked={selectedChannels.includes(channel.value)}
                  onChange={() => handleChannelToggle(channel.value)}
                  className="rounded border-gray-300 text-primary-600 focus:ring-primary-500"
                />
                <span className="ml-2 text-sm">{channel.displayName}</span>
              </label>
            ))}
          </div>
        </div>

        {/* Connected Data Sources */}
        <div className="bg-white p-4 rounded-lg border border-gray-200">
          <h3 className="font-semibold text-gray-900 mb-3">Connected Sources</h3>
          <div className="space-y-2">
            {dataSources.filter(ds => ds.isConnected).map(source => (
              <div key={source.id} className="flex items-center text-sm">
                <div className="w-2 h-2 bg-green-500 rounded-full mr-2"></div>
                <span>{source.name}</span>
              </div>
            ))}
            {dataSources.filter(ds => ds.isConnected).length === 0 && (
              <p className="text-sm text-gray-500">
                No data sources connected. Connect data sources to get better campaign recommendations.
              </p>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default ChatInterface;