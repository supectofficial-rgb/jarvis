using OysterFx.AppCore.Domain.Aggregates;

namespace Insurance.PageRuntimeService.AppCore.Domain.Sections.Entities;

public class UrlData : Entity<long>
{
    public long SectionId { get; private set; }
    public string Name { get; private set; } = null!;
    public string FilterJson { get; private set; } = null!;
    public string RelationType { get; private set; } = null!;
    public string ActionFields { get; private set; } = null!;
    public string FormFields { get; private set; } = null!;
    public string FlowFields { get; private set; } = null!;
    public string FormIdString { get; private set; } = null!;
    public string UserFields { get; private set; } = null!;
    public string StatusFields { get; private set; } = null!;
    public string LogFields { get; private set; } = null!;
    public string ActionRole { get; private set; } = null!;
    public string RelationCondition { get; private set; } = null!;
    public string AnotherRelationCondition { get; private set; } = null!;
    public string FromAnother { get; private set; } = null!;
    public string RoleId { get; private set; } = null!;
    public string IsPerson { get; private set; } = null!;
    public string Next { get; private set; } = null!;
    public int PermissionCheck { get; private set; }
    public int IsCycle { get; private set; }
    public int IsCustom { get; private set; }
    public int ThirdIdIsNeeded { get; private set; }
    public int FormOwner { get; private set; }
    public int IsOrderByDate { get; private set; }
    public int IsForCurrentDay { get; private set; }
    public int? FormTypeId { get; private set; }
    public int? FormId { get; private set; }
    public int UserInfo { get; private set; }
    public long? FormItemId { get; private set; }
    public int? FormItemType { get; private set; }
    public int IsLinkToMain { get; private set; }
    public string ConditionStatus { get; private set; } = null!;
    public string RelationStatus { get; private set; } = null!;
    public string ConditionRelationStatus { get; private set; } = null!;
    public int ConditionStatusOperator { get; private set; }
    public string Operat { get; private set; } = null!;
    public string ExtraRelation { get; private set; } = null!;

    protected UrlData() { }

    public static UrlData Create(
        long sectionId,
        string name,
        string filterJson,
        string relationType,
        string actionFields,
        string formFields,
        string flowFields,
        string formIdString,
        string userFields,
        string statusFields,
        string logFields,
        string actionRole,
        string relationCondition,
        string anotherRelationCondition,
        string fromAnother,
        string roleId,
        string isPerson,
        string next,
        int permissionCheck,
        int isCycle,
        int isCustom,
        int thirdIdIsNeeded,
        int formOwner,
        int isOrderByDate,
        int isForCurrentDay,
        int? formTypeId,
        int? formId,
        int userInfo,
        long? formItemId,
        int? formItemType,
        int isLinkToMain,
        string conditionStatus,
        string relationStatus,
        string conditionRelationStatus,
        int conditionStatusOperator,
        string operat,
        string extraRelation)
    {
        return new UrlData
        {
            SectionId = sectionId,
            Name = name,
            FilterJson = filterJson,
            RelationType = relationType,
            ActionFields = actionFields,
            FormFields = formFields,
            FlowFields = flowFields,
            FormIdString = formIdString,
            UserFields = userFields,
            StatusFields = statusFields,
            LogFields = logFields,
            ActionRole = actionRole,
            RelationCondition = relationCondition,
            AnotherRelationCondition = anotherRelationCondition,
            FromAnother = fromAnother,
            RoleId = roleId,
            IsPerson = isPerson,
            Next = next,
            PermissionCheck = permissionCheck,
            IsCycle = isCycle,
            IsCustom = isCustom,
            ThirdIdIsNeeded = thirdIdIsNeeded,
            FormOwner = formOwner,
            IsOrderByDate = isOrderByDate,
            IsForCurrentDay = isForCurrentDay,
            FormTypeId = formTypeId,
            FormId = formId,
            UserInfo = userInfo,
            FormItemId = formItemId,
            FormItemType = formItemType,
            IsLinkToMain = isLinkToMain,
            ConditionStatus = conditionStatus,
            RelationStatus = relationStatus,
            ConditionRelationStatus = conditionRelationStatus,
            ConditionStatusOperator = conditionStatusOperator,
            Operat = operat,
            ExtraRelation = extraRelation
        };
    }

    public void Update(
        long sectionId,
        string name,
        string filterJson,
        string relationType,
        string actionFields,
        string formFields,
        string flowFields,
        string formIdString,
        string userFields,
        string statusFields,
        string logFields,
        string actionRole,
        string relationCondition,
        string anotherRelationCondition,
        string fromAnother,
        string roleId,
        string isPerson,
        string next,
        int permissionCheck,
        int isCycle,
        int isCustom,
        int thirdIdIsNeeded,
        int formOwner,
        int isOrderByDate,
        int isForCurrentDay,
        int? formTypeId,
        int? formId,
        int userInfo,
        long? formItemId,
        int? formItemType,
        int isLinkToMain,
        string conditionStatus,
        string relationStatus,
        string conditionRelationStatus,
        int conditionStatusOperator,
        string operat,
        string extraRelation)
    {
        SectionId = sectionId;
        Name = name;
        FilterJson = filterJson;
        RelationType = relationType;
        ActionFields = actionFields;
        FormFields = formFields;
        FlowFields = flowFields;
        FormIdString = formIdString;
        UserFields = userFields;
        StatusFields = statusFields;
        LogFields = logFields;
        ActionRole = actionRole;
        RelationCondition = relationCondition;
        AnotherRelationCondition = anotherRelationCondition;
        FromAnother = fromAnother;
        RoleId = roleId;
        IsPerson = isPerson;
        Next = next;
        PermissionCheck = permissionCheck;
        IsCycle = isCycle;
        IsCustom = isCustom;
        ThirdIdIsNeeded = thirdIdIsNeeded;
        FormOwner = formOwner;
        IsOrderByDate = isOrderByDate;
        IsForCurrentDay = isForCurrentDay;
        FormTypeId = formTypeId;
        FormId = formId;
        UserInfo = userInfo;
        FormItemId = formItemId;
        FormItemType = formItemType;
        IsLinkToMain = isLinkToMain;
        ConditionStatus = conditionStatus;
        RelationStatus = relationStatus;
        ConditionRelationStatus = conditionRelationStatus;
        ConditionStatusOperator = conditionStatusOperator;
        Operat = operat;
        ExtraRelation = extraRelation;
    }
}


