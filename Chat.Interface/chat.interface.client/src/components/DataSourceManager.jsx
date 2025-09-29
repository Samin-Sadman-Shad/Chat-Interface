import React, { useState, useEffect } from 'react';
import { Plus, Check, X, Settings, RefreshCw } from 'lucide-react';
import axios from 'axios';

const DataSourceManager = ({ dataSources, setDataSources }) => {
  const [showConnectModal, setShowConnectModal] = useState(false);
  const [availableTypes, setAvailableTypes] = useState([]);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    fetchDataSources();
    fetchDataSourceTypes();
  }, []);

  const fetchDataSources = async () => {
    try {
      const response = await axios.get('/api/datasources');
      setDataSources(response.data);
    } catch (error) {
      console.error('Error fetching data sources:', error);
    }
  };

  const fetchDataSourceTypes = async () => {
    try {
      const response = await axios.get('/api/datasources/types');
      setAvailableTypes(response.data);
    } catch (error) {
      console.error('Error fetching data source types:', error);
    }
  };

  const getTypeDisplayName = (type) => {
    const typeMap = {
      0: 'Google Tag Manager',
      1: 'Facebook Pixel',
      2: 'Google Ads',
      3: 'Facebook Page',
      4: 'Website Analytics',
      5: 'Shopify Store',
      6: 'Customer CRM',
      7: 'Twitter Page',
      8: 'Review Sites',
      9: 'Ad Manager'
    };
    return typeMap[type] || 'Unknown';
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex justify-between items-center">
        <div>
          <h2 className="text-2xl font-bold text-gray-900">Data Sources</h2>
          <p className="text-gray-600">
            Connect your marketing data sources to get personalized campaign recommendations.
          </p>
        </div>
        <button
          onClick={() => setShowConnectModal(true)}
          className="bg-primary-500 text-white px-4 py-2 rounded-lg hover:bg-primary-600 flex items-center space-x-2"
        >
          <Plus size={20} />
          <span>Connect Source</span>
        </button>
      </div>

      {/* Data Sources Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {dataSources.map(source => (
          <DataSourceCard key={source.id} source={source} getTypeDisplayName={getTypeDisplayName} />
        ))}
        
        {dataSources.length === 0 && (
          <div className="col-span-full text-center py-12">
            <div className="text-gray-400 mb-4">
              <Settings size={64} className="mx-auto" />
            </div>
            <h3 className="text-lg font-medium text-gray-900 mb-2">
              No Data Sources Connected
            </h3>
            <p className="text-gray-600 mb-4">
              Get started by connecting your first data source to unlock AI-powered campaign insights.
            </p>
            <button
              onClick={() => setShowConnectModal(true)}
              className="bg-primary-500 text-white px-6 py-3 rounded-lg hover:bg-primary-600"
            >
              Connect Your First Source
            </button>
          </div>
        )}
      </div>

      {/* Connect Modal */}
      {showConnectModal && (
        <ConnectDataSourceModal
          availableTypes={availableTypes}
          onClose={() => setShowConnectModal(false)}
          onSuccess={() => {
            setShowConnectModal(false);
            fetchDataSources();
          }}
        />
      )}
    </div>
  );
};

const DataSourceCard = ({ source, getTypeDisplayName }) => {
  return (
    <div className={`data-source-card ${source.isConnected ? 'connected' : ''}`}>
      <div className="flex items-start justify-between mb-4">
        <div>
          <h3 className="font-semibold text-gray-900">{source.name}</h3>
          <p className="text-sm text-gray-600">{getTypeDisplayName(source.type)}</p>
        </div>
        <div className={`w-3 h-3 rounded-full ${
          source.isConnected ? 'bg-green-500' : 'bg-gray-300'
        }`}></div>
      </div>
      
      <div className="space-y-2">
        <div className="flex items-center justify-between text-sm">
          <span className="text-gray-600">Status</span>
          <span className={`flex items-center space-x-1 ${
            source.isConnected ? 'text-green-600' : 'text-gray-500'
          }`}>
            {source.isConnected ? <Check size={14} /> : <X size={14} />}
            <span>{source.isConnected ? 'Connected' : 'Not Connected'}</span>
          </span>
        </div>
        
        {source.isConnected && source.lastSyncAt && (
          <div className="flex items-center justify-between text-sm">
            <span className="text-gray-600">Last Sync</span>
            <span className="text-gray-900">
              {new Date(source.lastSyncAt).toLocaleDateString()}
            </span>
          </div>
        )}
      </div>
      
      <div className="mt-4 pt-4 border-t border-gray-200">
        <button className="text-primary-500 text-sm hover:text-primary-600 flex items-center space-x-1">
          <Settings size={14} />
          <span>Configure</span>
        </button>
      </div>
    </div>
  );
};

const ConnectDataSourceModal = ({ availableTypes, onClose, onSuccess }) => {
  const [selectedType, setSelectedType] = useState(null);
  const [name, setName] = useState('');
  const [configuration, setConfiguration] = useState({});
  const [isConnecting, setIsConnecting] = useState(false);

  const handleConnect = async () => {
    if (!selectedType || !name.trim()) return;

    setIsConnecting(true);
    try {
      await axios.post('/api/datasources/connect', {
        type: selectedType.value,
        name: name.trim(),
        configuration
      });
      onSuccess();
    } catch (error) {
      console.error('Error connecting data source:', error);
    } finally {
      setIsConnecting(false);
    }
  };

  const renderConfigurationFields = () => {
    if (!selectedType) return null;

    switch (selectedType.name) {
      case 'GTM':
        return (
          <input
            type="text"
            placeholder="Container ID (GTM-XXXXXXX)"
            value={configuration.containerId || ''}
            onChange={(e) => setConfiguration({...configuration, containerId: e.target.value})}
            className="w-full border border-gray-300 rounded-lg px-3 py-2"
          />
        );
      case 'FacebookPixel':
        return (
          <input
            type="text"
            placeholder="Pixel ID"
            value={configuration.pixelId || ''}
            onChange={(e) => setConfiguration({...configuration, pixelId: e.target.value})}
            className="w-full border border-gray-300 rounded-lg px-3 py-2"
          />
        );
      case 'Shopify':
        return (
          <div className="space-y-3">
            <input
              type="text"
              placeholder="Shop Name (myshop.myshopify.com)"
              value={configuration.shopName || ''}
              onChange={(e) => setConfiguration({...configuration, shopName: e.target.value})}
              className="w-full border border-gray-300 rounded-lg px-3 py-2"
            />
            <input
              type="text"
              placeholder="API Key"
              value={configuration.apiKey || ''}
              onChange={(e) => setConfiguration({...configuration, apiKey: e.target.value})}
              className="w-full border border-gray-300 rounded-lg px-3 py-2"
            />
          </div>
        );
      default:
        return (
          <input
            type="text"
            placeholder="API Key or Connection String"
            value={configuration.apiKey || ''}
            onChange={(e) => setConfiguration({...configuration, apiKey: e.target.value})}
            className="w-full border border-gray-300 rounded-lg px-3 py-2"
          />
        );
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg p-6 w-full max-w-md">
        <div className="flex justify-between items-center mb-4">
          <h3 className="text-lg font-semibold">Connect Data Source</h3>
          <button onClick={onClose} className="text-gray-500 hover:text-gray-700">
            <X size={20} />
          </button>
        </div>

        <div className="space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Data Source Type
            </label>
            <select
              value={selectedType?.value || ''}
              onChange={(e) => {
                const type = availableTypes.find(t => t.value === parseInt(e.target.value));
                setSelectedType(type);
                setName(type?.displayName || '');
              }}
              className="w-full border border-gray-300 rounded-lg px-3 py-2"
            >
              <option value="">Select a data source type</option>
              {availableTypes.map(type => (
                <option key={type.value} value={type.value}>
                  {type.displayName}
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Name
            </label>
            <input
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="Enter a name for this connection"
              className="w-full border border-gray-300 rounded-lg px-3 py-2"
            />
          </div>

          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Configuration
            </label>
            {renderConfigurationFields()}
          </div>

          <div className="flex space-x-3 pt-4">
            <button
              onClick={onClose}
              className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50"
            >
              Cancel
            </button>
            <button
              onClick={handleConnect}
              disabled={!selectedType || !name.trim() || isConnecting}
              className="flex-1 bg-primary-500 text-white px-4 py-2 rounded-lg hover:bg-primary-600 disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center space-x-2"
            >
              {isConnecting && <RefreshCw size={16} className="animate-spin" />}
              <span>{isConnecting ? 'Connecting...' : 'Connect'}</span>
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default DataSourceManager;