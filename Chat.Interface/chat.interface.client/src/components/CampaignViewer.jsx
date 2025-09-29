import React, { useState } from 'react';
import { Play, Pause, Eye, Calendar, Users, MessageCircle, Target, BarChart3, Edit } from 'lucide-react';

const CampaignViewer = ({ campaigns }) => {
  const [selectedCampaign, setSelectedCampaign] = useState(null);

  const getStatusColor = (status) => {
    const colors = {
      Draft: 'bg-gray-100 text-gray-800',
      Scheduled: 'bg-blue-100 text-blue-800',
      Running: 'bg-green-100 text-green-800',
      Completed: 'bg-purple-100 text-purple-800',
      Failed: 'bg-red-100 text-red-800',
      Paused: 'bg-yellow-100 text-yellow-800'
    };
    return colors[status] || colors.Draft;
  };

  const formatCurrency = (value) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(value);
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex justify-between items-center">
        <div>
          <h2 className="text-2xl font-bold text-gray-900">Campaigns</h2>
          <p className="text-gray-600">
            View and manage your AI-generated marketing campaigns.
          </p>
        </div>
        <div className="flex space-x-4">
          <select className="border border-gray-300 rounded-lg px-3 py-2">
            <option>All Statuses</option>
            <option>Draft</option>
            <option>Scheduled</option>
            <option>Running</option>
            <option>Completed</option>
          </select>
        </div>
      </div>

      {campaigns.length === 0 ? (
        <div className="text-center py-12">
          <div className="text-gray-400 mb-4">
            <Target size={64} className="mx-auto" />
          </div>
          <h3 className="text-lg font-medium text-gray-900 mb-2">
            No Campaigns Yet
          </h3>
          <p className="text-gray-600 mb-4">
            Start a conversation in the chat to generate your first AI-powered campaign.
          </p>
        </div>
      ) : (
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {/* Campaign List */}
          <div className="lg:col-span-2 space-y-4">
            {campaigns.map(campaign => (
              <CampaignCard
                key={campaign.id}
                campaign={campaign}
                onSelect={() => setSelectedCampaign(campaign)}
                isSelected={selectedCampaign?.id === campaign.id}
              />
            ))}
          </div>

          {/* Campaign Details */}
          <div className="lg:col-span-1">
            {selectedCampaign ? (
              <CampaignDetails campaign={selectedCampaign} />
            ) : (
              <div className="bg-white p-6 rounded-lg border border-gray-200 text-center">
                <Target size={48} className="mx-auto text-gray-400 mb-4" />
                <p className="text-gray-600">
                  Select a campaign to view details
                </p>
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
};

const CampaignCard = ({ campaign, onSelect, isSelected }) => {
  return (
    <div
      onClick={onSelect}
      className={`bg-white p-6 rounded-lg border cursor-pointer transition-all ${
        isSelected 
          ? 'border-primary-500 ring-2 ring-primary-200' 
          : 'border-gray-200 hover:border-gray-300'
      }`}
    >
      <div className="flex items-start justify-between mb-4">
        <div className="flex-1">
          <h3 className="text-lg font-semibold text-gray-900 mb-2">
            {campaign.name}
          </h3>
          <p className="text-gray-600 text-sm line-clamp-2">
            {campaign.message}
          </p>
        </div>
        <span className={`px-2 py-1 rounded-full text-xs font-medium ${getStatusColor(campaign.status)}`}>
          {campaign.status}
        </span>
      </div>

      <div className="grid grid-cols-2 gap-4 mb-4">
        <div className="flex items-center space-x-2 text-sm text-gray-600">
          <Users size={16} />
          <span>{campaign.audience.estimatedSize.toLocaleString()} people</span>
        </div>
        <div className="flex items-center space-x-2 text-sm text-gray-600">
          <Calendar size={16} />
          <span>
            {campaign.timing.optimalTime 
              ? new Date(campaign.timing.optimalTime).toLocaleDateString()
              : 'Not scheduled'}
          </span>
        </div>
      </div>

      <div className="flex items-center justify-between">
        <div className="flex space-x-2">
          {campaign.channels.map(channel => (
            <span
              key={channel}
              className="px-2 py-1 bg-gray-100 text-gray-700 rounded text-xs"
            >
              {channel}
            </span>
          ))}
        </div>
        <div className="flex space-x-2">
          {campaign.status === 'Draft' && (
            <button className="text-green-600 hover:text-green-700">
              <Play size={16} />
            </button>
          )}
          {campaign.status === 'Running' && (
            <button className="text-yellow-600 hover:text-yellow-700">
              <Pause size={16} />
            </button>
          )}
          <button className="text-gray-600 hover:text-gray-700">
            <Edit size={16} />
          </button>
        </div>
      </div>
    </div>
  );
};

const CampaignDetails = ({ campaign }) => {
  return (
    <div className="bg-white p-6 rounded-lg border border-gray-200 space-y-6">
      <div>
        <h3 className="text-lg font-semibold text-gray-900 mb-2">
          Campaign Details
        </h3>
        <span className={`px-2 py-1 rounded-full text-xs font-medium ${getStatusColor(campaign.status)}`}>
          {campaign.status}
        </span>
      </div>

      {/* Message Preview */}
      <div>
        <h4 className="font-medium text-gray-900 mb-2 flex items-center">
          <MessageCircle size={16} className="mr-2" />
          Message
        </h4>
        <div className="bg-gray-50 p-3 rounded-lg text-sm">
          {campaign.message}
        </div>
      </div>

      {/* Audience */}
      <div>
        <h4 className="font-medium text-gray-900 mb-2 flex items-center">
          <Users size={16} className="mr-2" />
          Audience
        </h4>
        <div className="space-y-2 text-sm">
          <div className="flex justify-between">
            <span className="text-gray-600">Segment:</span>
            <span className="font-medium">{campaign.audience.name}</span>
          </div>
          <div className="flex justify-between">
            <span className="text-gray-600">Size:</span>
            <span className="font-medium">{campaign.audience.estimatedSize.toLocaleString()}</span>
          </div>
          <div className="mt-3">
            <span className="text-gray-600 text-xs">Criteria:</span>
            <div className="mt-1 space-y-1">
              {Object.entries(campaign.audience.criteria).map(([key, value]) => (
                <div key={key} className="flex justify-between text-xs">
                  <span className="text-gray-500 capitalize">{key.replace(/([A-Z])/g, ' $1')}:</span>
                  <span className="text-gray-700">
                    {Array.isArray(value) ? value.join(', ') : String(value)}
                  </span>
                </div>
              ))}
            </div>
          </div>
        </div>
      </div>

      {/* Timing */}
      <div>
        <h4 className="font-medium text-gray-900 mb-2 flex items-center">
          <Calendar size={16} className="mr-2" />
          Timing
        </h4>
        <div className="space-y-2 text-sm">
          <div className="flex justify-between">
            <span className="text-gray-600">Optimal Time:</span>
            <span className="font-medium">
              {campaign.timing.optimalTime 
                ? new Date(campaign.timing.optimalTime).toLocaleString()
                : 'Not set'}
            </span>
          </div>
          <div className="flex justify-between">
            <span className="text-gray-600">Time Zone:</span>
            <span className="font-medium">{campaign.timing.timeZone}</span>
          </div>
          <div className="flex justify-between">
            <span className="text-gray-600">Preferred Days:</span>
            <span className="font-medium text-xs">
              {campaign.timing.preferredDays.join(', ')}
            </span>
          </div>
        </div>
      </div>

      {/* Channels */}
      <div>
        <h4 className="font-medium text-gray-900 mb-2 flex items-center">
          <Target size={16} className="mr-2" />
          Channels
        </h4>
        <div className="flex flex-wrap gap-2">
          {campaign.channels.map(channel => (
            <span
              key={channel}
              className="px-2 py-1 bg-primary-100 text-primary-700 rounded text-xs font-medium"
            >
              {channel}
            </span>
          ))}
        </div>
      </div>

      {/* Metadata */}
      {campaign.metadata.confidence && (
        <div>
          <h4 className="font-medium text-gray-900 mb-2 flex items-center">
            <BarChart3 size={16} className="mr-2" />
            AI Confidence
          </h4>
          <div className="flex items-center space-x-2">
            <div className="flex-1 bg-gray-200 rounded-full h-2">
              <div
                className="bg-green-500 h-2 rounded-full"
                style={{ width: `${campaign.metadata.confidence * 100}%` }}
              ></div>
            </div>
            <span className="text-sm font-medium">
              {Math.round(campaign.metadata.confidence * 100)}%
            </span>
          </div>
        </div>
      )}

      {/* Actions */}
      <div className="pt-4 border-t border-gray-200 space-y-2">
        {campaign.status === 'Draft' && (
          <button className="w-full bg-green-500 text-white py-2 rounded-lg hover:bg-green-600 flex items-center justify-center space-x-2">
            <Play size={16} />
            <span>Launch Campaign</span>
          </button>
        )}
        {campaign.status === 'Running' && (
          <button className="w-full bg-yellow-500 text-white py-2 rounded-lg hover:bg-yellow-600 flex items-center justify-center space-x-2">
            <Pause size={16} />
            <span>Pause Campaign</span>
          </button>
        )}
        <button className="w-full border border-gray-300 text-gray-700 py-2 rounded-lg hover:bg-gray-50 flex items-center justify-center space-x-2">
          <Eye size={16} />
          <span>Preview</span>
        </button>
        <button className="w-full border border-gray-300 text-gray-700 py-2 rounded-lg hover:bg-gray-50 flex items-center justify-center space-x-2">
          <Edit size={16} />
          <span>Edit Campaign</span>
        </button>
      </div>
    </div>
  );
};

// Helper function (should be moved to utils if created)
const getStatusColor = (status) => {
  const colors = {
    Draft: 'bg-gray-100 text-gray-800',
    Scheduled: 'bg-blue-100 text-blue-800',
    Running: 'bg-green-100 text-green-800',
    Completed: 'bg-purple-100 text-purple-800',
    Failed: 'bg-red-100 text-red-800',
    Paused: 'bg-yellow-100 text-yellow-800'
  };
  return colors[status] || colors.Draft;
};

export default CampaignViewer;