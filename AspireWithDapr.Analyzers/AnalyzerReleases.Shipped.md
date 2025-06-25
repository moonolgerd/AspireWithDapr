## Release 1.0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
DAPR001 | Interface | Error | Actor interface should inherit from IActor
DAPR002 | Serialization | Warning | Enum members in Actor types should use EnumMember attribute
DAPR003 | Serialization | Info | Consider using JsonPropertyName for property name consistency
DAPR004 | Serialization | Warning | Complex types used in Actor methods need serialization attributes
DAPR005 | Serialization | Warning | Actor method parameter needs proper serialization attributes
DAPR006 | Serialization | Warning | Actor method return type needs proper serialization attributes
DAPR007 | Serialization | Warning | Collection types in Actor methods need element type validation
DAPR008 | Serialization | Warning | Record types should use DataContract and DataMember attributes for Actor serialization
DAPR009 | Serialization | Error | Actor class implementation should implement an interface that inherits from IActor

