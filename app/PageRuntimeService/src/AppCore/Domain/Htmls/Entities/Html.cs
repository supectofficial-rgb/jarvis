using OysterFx.AppCore.Domain.Aggregates;

namespace Insurance.PageRuntimeService.AppCore.Domain.Htmls.Entities;

public class Html : AggregateRoot
{
    public string DefaultHtmls { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string AppMeta { get; private set; } = null!;
    public string PoseMeta { get; private set; } = null!;
    public string AppType { get; private set; } = null!;
    public string Image { get; private set; } = null!;
    public string FormLink { get; private set; } = null!;
    public string PartialView { get; private set; } = null!;
    public string IsPublic { get; private set; } = null!;
    public int Multilayer { get; private set; }
    public string DataField { get; private set; } = null!;
    public string PoseField { get; private set; } = null!;
    public int ChildCapable { get; private set; }
    public long? ParentId { get; private set; }
    public long? LayoutId { get; private set; }

    protected Html() { }

    public static Html Create(
        string title,
        string defaultHtmls,
        string appMeta,
        string poseMeta,
        string appType,
        string image,
        string formLink,
        string partialView,
        string isPublic,
        int multilayer,
        string dataField,
        string poseField,
        int childCapable,
        long? parentId,
        long? layoutId)
    {
        return new Html
        {
            Title = title,
            DefaultHtmls = defaultHtmls,
            AppMeta = appMeta,
            PoseMeta = poseMeta,
            AppType = appType,
            Image = image,
            FormLink = formLink,
            PartialView = partialView,
            IsPublic = isPublic,
            Multilayer = multilayer,
            DataField = dataField,
            PoseField = poseField,
            ChildCapable = childCapable,
            ParentId = parentId,
            LayoutId = layoutId
        };
    }

    public void Update(
        string title,
        string defaultHtmls,
        string appMeta,
        string poseMeta,
        string appType,
        string image,
        string formLink,
        string partialView,
        string isPublic,
        int multilayer,
        string dataField,
        string poseField,
        int childCapable,
        long? parentId,
        long? layoutId)
    {
        Title = title;
        DefaultHtmls = defaultHtmls;
        AppMeta = appMeta;
        PoseMeta = poseMeta;
        AppType = appType;
        Image = image;
        FormLink = formLink;
        PartialView = partialView;
        IsPublic = isPublic;
        Multilayer = multilayer;
        DataField = dataField;
        PoseField = poseField;
        ChildCapable = childCapable;
        ParentId = parentId;
        LayoutId = layoutId;
    }
}


