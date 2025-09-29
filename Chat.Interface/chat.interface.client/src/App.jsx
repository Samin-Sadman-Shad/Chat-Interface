import React, { useState, useEffect } from 'react';
import { HubConnectionBuilder } from '@microsoft/signalr';
import ChatInterface from './components/ChatInterface';
import DataSourceManager from './components/DataSourceManager';
import CampaignViewer from './components/CampaignViewer';
import { MessageSquare, Database, Target } from 'lucide-react';

function App() {
  const [activeTab, setActiveTab] = useState('chat');
  const [connection, setConnection] = useState(null);
  const [dataSources, setDataSources] = useState([]);
  const [campaigns, setCampaigns] = useState([]);

  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
      .withUrl('/chathub')
      .build();

    setConnection(newConnection);

    newConnection.start()
      .then(() => console.log('Connected to SignalR hub'))
      .catch(err => console.error('SignalR connection error:', err));

    return () => newConnection.stop();
  }, []);

  const handleCampaignGenerated = (campaign) => {
    setCampaigns(prev => [...prev, campaign]);
    setActiveTab('campaigns');
  };

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex items-center justify-between h-16">
            <div className="flex items-center space-x-4">
              <h1 className="text-2xl font-bold text-gray-900">
                Campaign Intelligence
              </h1>
            </div>
            <nav className="flex space-x-8">
              <button
                onClick={() => setActiveTab('chat')}
                className={`flex items-center space-x-2 px-3 py-2 rounded-md text-sm font-medium ${
                  activeTab === 'chat'
                    ? 'bg-primary-100 text-primary-700'
                    : 'text-gray-500 hover:text-gray-700'
                }`}
              >
                <MessageSquare size={16} />
                <span>Chat</span>
              </button>
              <button
                onClick={() => setActiveTab('datasources')}
                className={`flex items-center space-x-2 px-3 py-2 rounded-md text-sm font-medium ${
                  activeTab === 'datasources'
                    ? 'bg-primary-100 text-primary-700'
                    : 'text-gray-500 hover:text-gray-700'
                }`}
              >
                <Database size={16} />
                <span>Data Sources</span>
              </button>
              <button
                onClick={() => setActiveTab('campaigns')}
                className={`flex items-center space-x-2 px-3 py-2 rounded-md text-sm font-medium ${
                  activeTab === 'campaigns'
                    ? 'bg-primary-100 text-primary-700'
                    : 'text-gray-500 hover:text-gray-700'
                }`}
              >
                <Target size={16} />
                <span>Campaigns</span>
              </button>
            </nav>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {activeTab === 'chat' && (
          <ChatInterface 
            connection={connection}
            dataSources={dataSources}
            onCampaignGenerated={handleCampaignGenerated}
          />
        )}
        {activeTab === 'datasources' && (
          <DataSourceManager 
            dataSources={dataSources}
            setDataSources={setDataSources}
          />
        )}
        {activeTab === 'campaigns' && (
          <CampaignViewer campaigns={campaigns} />
        )}
      </main>
    </div>
  );
}

export default App;