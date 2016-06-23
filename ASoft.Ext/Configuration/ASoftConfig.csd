<?xml version="1.0" encoding="utf-8"?>
<configurationSectionModel xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="18501e18-bd51-43e4-a6ca-b50803b300d4" namespace="ASoft.Configuration" xmlSchemaNamespace="urn:ASoft.Configuration" xmlns="http://schemas.microsoft.com/dsltools/ConfigurationSectionDesigner">
  <typeDefinitions>
    <externalType name="String" namespace="System" />
    <externalType name="Boolean" namespace="System" />
    <externalType name="Int32" namespace="System" />
    <externalType name="Int64" namespace="System" />
    <externalType name="Single" namespace="System" />
    <externalType name="Double" namespace="System" />
    <externalType name="DateTime" namespace="System" />
    <externalType name="TimeSpan" namespace="System" />
  </typeDefinitions>
  <configurationElements>
    <configurationSection name="ASoftConfig" codeGenOptions="Singleton, XmlnsProperty" xmlSectionName="asoftConfig">
      <elementProperties>
        <elementProperty name="EventQueue" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="eventQueue" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/18501e18-bd51-43e4-a6ca-b50803b300d4/MessageQueueConfigurationElement" />
          </type>
        </elementProperty>
        <elementProperty name="CommandQueue" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="commandQueue" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/18501e18-bd51-43e4-a6ca-b50803b300d4/MessageQueueConfigurationElement" />
          </type>
        </elementProperty>
        <elementProperty name="Services" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="services" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/18501e18-bd51-43e4-a6ca-b50803b300d4/ServiceElementCollection" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationSection>
    <configurationElement name="MessageQueueConfigurationElement">
      <attributeProperties>
        <attributeProperty name="ConnectionUri" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="connectionUri" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/18501e18-bd51-43e4-a6ca-b50803b300d4/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="ExchangeName" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="exchangeName" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/18501e18-bd51-43e4-a6ca-b50803b300d4/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="QueueName" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="queueName" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/18501e18-bd51-43e4-a6ca-b50803b300d4/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
    <configurationElementCollection name="ServiceElementCollection" xmlItemName="service" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods, ICollection">
      <itemType>
        <configurationElementMoniker name="/18501e18-bd51-43e4-a6ca-b50803b300d4/ServiceElement" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="ServiceElement">
      <attributeProperties>
        <attributeProperty name="Type" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="type" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/18501e18-bd51-43e4-a6ca-b50803b300d4/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
      <elementProperties>
        <elementProperty name="Settings" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="settings" isReadOnly="false">
          <type>
            <configurationElementCollectionMoniker name="/18501e18-bd51-43e4-a6ca-b50803b300d4/SettingElementCollection" />
          </type>
        </elementProperty>
        <elementProperty name="LocalCommandQueue" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="localCommandQueue" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/18501e18-bd51-43e4-a6ca-b50803b300d4/MessageQueueConfigurationElement" />
          </type>
        </elementProperty>
        <elementProperty name="LocalEventQueue" isRequired="false" isKey="false" isDefaultCollection="false" xmlName="localEventQueue" isReadOnly="false">
          <type>
            <configurationElementMoniker name="/18501e18-bd51-43e4-a6ca-b50803b300d4/MessageQueueConfigurationElement" />
          </type>
        </elementProperty>
      </elementProperties>
    </configurationElement>
    <configurationElementCollection name="SettingElementCollection" xmlItemName="settingElement" codeGenOptions="Indexer, AddMethod, RemoveMethod, GetItemMethods">
      <itemType>
        <configurationElementMoniker name="/18501e18-bd51-43e4-a6ca-b50803b300d4/SettingElement" />
      </itemType>
    </configurationElementCollection>
    <configurationElement name="SettingElement">
      <attributeProperties>
        <attributeProperty name="Key" isRequired="true" isKey="true" isDefaultCollection="false" xmlName="key" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/18501e18-bd51-43e4-a6ca-b50803b300d4/String" />
          </type>
        </attributeProperty>
        <attributeProperty name="Value" isRequired="true" isKey="false" isDefaultCollection="false" xmlName="value" isReadOnly="false">
          <type>
            <externalTypeMoniker name="/18501e18-bd51-43e4-a6ca-b50803b300d4/String" />
          </type>
        </attributeProperty>
      </attributeProperties>
    </configurationElement>
  </configurationElements>
  <propertyValidators>
    <validators />
  </propertyValidators>
</configurationSectionModel>