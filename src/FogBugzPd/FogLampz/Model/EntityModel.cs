
using System;
using System.Collections.Generic;
using FogLampz.Attributes;

namespace FogLampz.Model
{
  [EntityApiInfo(CreateCommand = "newArea", ListCommand = "listAreas", Root = "areas", Element = "area")]
  public partial class Area : FogBugzEntityBase
  {
    public Area() {}
	public Area(IDictionary<string,string> fields) : base(fields) {  }

		[PropertyMap("ixArea", ConversionStrategy.Integer)]
	public int? Index {get;set;}
		[PropertyMap("sArea", ConversionStrategy.String)]
	public string Name {get;set;}
		[PropertyMap("nType", ConversionStrategy.Integer)]
	public int? AreaType {get;set;}
		[PropertyMap("ixProject", ConversionStrategy.Integer)]
	public int? IndexProject {get;set;}
		[PropertyMap("sProject", ConversionStrategy.String)]
	public string ProjectName {get;set;}
		[PropertyMap("ixPersonOwner", ConversionStrategy.Integer)]
	public int? IndexPersonOwner {get;set;}
		[PropertyMap("sPersonOwner", ConversionStrategy.String)]
	public string PersonOwnerName {get;set;}
		[PropertyMap("cDoc", ConversionStrategy.Integer)]
	public int? Doc {get;set;}
	  }
  [EntityApiInfo(CreateCommand = "newFixFor", ListCommand = "listFixFors", Root = "fixfors", Element = "fixfor")]
  public partial class FixFor : FogBugzEntityBase
  {
    public FixFor() {}
	public FixFor(IDictionary<string,string> fields) : base(fields) {  }

		[PropertyMap("ixFixFor", ConversionStrategy.Integer)]
	public int? Index {get;set;}
		[PropertyMap("sFixFor", ConversionStrategy.String)]
	public string Name {get;set;}
		[PropertyMap("fDeleted", ConversionStrategy.Boolean)]
	public bool Deleted {get;set;}
		[PropertyMap("fReallyDeleted", ConversionStrategy.Boolean)]
	public bool ReallyDeleted {get;set;}
		[PropertyMap("dt", ConversionStrategy.DateTime)]
	public DateTime? Date {get;set;}
		[PropertyMap("dtStart", ConversionStrategy.DateTime)]
	public DateTime? DateStart {get;set;}
		[PropertyMap("dtRelease", ConversionStrategy.DateTime)]
	public DateTime? DateRelease {get;set;}
		[PropertyMap("ixProject", ConversionStrategy.Integer)]
	public int? IndexProject {get;set;}
		[PropertyMap("sProject", ConversionStrategy.String)]
	public string ProjectName {get;set;}
		[PropertyMap("sStartNote", ConversionStrategy.String)]
	public string Note {get;set;}
		[PropertyMap("setixFixForDependency", ConversionStrategy.Integer)]
	public int? SetIndexFixForDependency {get;set;}
		[PropertyMap("fAssignable", ConversionStrategy.Boolean)]
	public bool Assignable {get;set;}
	  }
  [EntityApiInfo(CreateCommand = "", ListCommand = "listStatuses", Root = "statuses", Element = "status")]
  public partial class Status : FogBugzEntityBase
  {
    public Status() {}
	public Status(IDictionary<string,string> fields) : base(fields) {  }

		[PropertyMap("sStatus", ConversionStrategy.String)]
	public string Name {get;set;}
		[PropertyMap("ixStatus", ConversionStrategy.Integer)]
	public int? Index {get;set;}
		[PropertyMap("ixCategory", ConversionStrategy.Integer)]
	public int? IndexCategory {get;set;}
		[PropertyMap("iOrder", ConversionStrategy.Integer)]
	public int? Order {get;set;}
		[PropertyMap("fResolved", ConversionStrategy.Boolean)]
	public bool Resolved {get;set;}
		[PropertyMap("fDeleted", ConversionStrategy.Boolean)]
	public bool Deleted {get;set;}
		[PropertyMap("fWorkDone", ConversionStrategy.Boolean)]
	public bool WorkDone {get;set;}
		[PropertyMap("fDuplicate", ConversionStrategy.Boolean)]
	public bool Duplicate {get;set;}
	  }
  [EntityApiInfo(CreateCommand = "", ListCommand = "listFilters", Root = "filters", Element = "filter")]
  public partial class Filter : FogBugzEntityBase
  {
    public Filter() {}
	public Filter(IDictionary<string,string> fields) : base(fields) {  }

		[PropertyMap("sName", ConversionStrategy.String)]
	public string Name {get;set;}
		[PropertyMap("type", ConversionStrategy.String)]
	public string FilterTypeName {get;set;}
		[PropertyMap("sFilter", ConversionStrategy.String)]
	public string Content {get;set;}
		[PropertyMap("status", ConversionStrategy.String)]
	public string Status {get;set;}
	  }
  [EntityApiInfo(CreateCommand = "", ListCommand = "listMailboxes", Root = "mailboxes", Element = "mailbox")]
  public partial class Mailbox : FogBugzEntityBase
  {
    public Mailbox() {}
	public Mailbox(IDictionary<string,string> fields) : base(fields) {  }

		[PropertyMap("ixMailbox", ConversionStrategy.Integer)]
	public int? Index {get;set;}
		[PropertyMap("sEmail", ConversionStrategy.String)]
	public string Email {get;set;}
		[PropertyMap("sEmailUser", ConversionStrategy.String)]
	public string EmailUser {get;set;}
		[PropertyMap("sTemplate", ConversionStrategy.String)]
	public string Template {get;set;}
	  }
  [EntityApiInfo(CreateCommand = "", ListCommand = "listPeople", Root = "people", Element = "person")]
  public partial class Person : FogBugzEntityBase
  {
    public Person() {}
	public Person(IDictionary<string,string> fields) : base(fields) {  }

		[PropertyMap("ixPerson", ConversionStrategy.Integer)]
	public int? Index {get;set;}
		[PropertyMap("sEmail", ConversionStrategy.String)]
	public string Email {get;set;}
		[PropertyMap("sFrom", ConversionStrategy.String)]
	public string From {get;set;}
		[PropertyMap("sFullName", ConversionStrategy.String)]
	public string Name {get;set;}
		[PropertyMap("fCommunity", ConversionStrategy.Boolean)]
	public bool Community {get;set;}
		[PropertyMap("fAdministrator", ConversionStrategy.Boolean)]
	public bool Administrator {get;set;}
		[PropertyMap("fVirtual", ConversionStrategy.Boolean)]
	public bool Virtual {get;set;}
		[PropertyMap("fNotify", ConversionStrategy.Boolean)]
	public bool Notify {get;set;}
		[PropertyMap("sHomepage", ConversionStrategy.String)]
	public string Homepage {get;set;}
		[PropertyMap("sLocale", ConversionStrategy.String)]
	public string Locale {get;set;}
		[PropertyMap("sLanguage", ConversionStrategy.String)]
	public string Language {get;set;}
		[PropertyMap("sTimeZoneKey", ConversionStrategy.String)]
	public string TimeZoneKey {get;set;}
		[PropertyMap("sLDAPUid", ConversionStrategy.String)]
	public string LdapUserId {get;set;}
		[PropertyMap("fPaletteExpanded", ConversionStrategy.Boolean)]
	public bool PaletteExpanded {get;set;}
		[PropertyMap("fRecurseBugChildren", ConversionStrategy.Boolean)]
	public bool RecurseBugChildren {get;set;}
		[PropertyMap("dtLastActivity", ConversionStrategy.DateTime)]
	public DateTime? DateLastActivity {get;set;}
		[PropertyMap("ixBugWorkingOn", ConversionStrategy.Integer)]
	public int? IndexBugWorkingOn {get;set;}
	  }
  [EntityApiInfo(CreateCommand = "", ListCommand = "listPriorities", Root = "priorities", Element = "priority")]
  public partial class Priority : FogBugzEntityBase
  {
    public Priority() {}
	public Priority(IDictionary<string,string> fields) : base(fields) {  }

		[PropertyMap("ixPriority", ConversionStrategy.Integer)]
	public int? Index {get;set;}
		[PropertyMap("sPriority", ConversionStrategy.String)]
	public string Name {get;set;}
		[PropertyMap("fDefault", ConversionStrategy.Boolean)]
	public bool Default {get;set;}
	  }
  [EntityApiInfo(CreateCommand = "newProject", ListCommand = "listProjects", Root = "projects", Element = "project")]
  public partial class Project : FogBugzEntityBase
  {
    public Project() {}
	public Project(IDictionary<string,string> fields) : base(fields) {  }

		[PropertyMap("ixProject", ConversionStrategy.Integer)]
	public int? Index {get;set;}
		[PropertyMap("sProject", ConversionStrategy.String)]
	public string Name {get;set;}
		[PropertyMap("ixPersonowner", ConversionStrategy.Integer)]
	public int? IndexOwner {get;set;}
		[PropertyMap("ixPersonPrimaryContact", ConversionStrategy.Integer)]
	public int? IndexPersonPrimaryContact {get;set;}
		[PropertyMap("sEmail", ConversionStrategy.String)]
	public string Email {get;set;}
		[PropertyMap("sPhone", ConversionStrategy.String)]
	public string Phone {get;set;}
		[PropertyMap("fInbox", ConversionStrategy.Boolean)]
	public bool Inbox {get;set;}
		[PropertyMap("fDeleted", ConversionStrategy.Boolean)]
	public bool Deleted {get;set;}
		[PropertyMap("ixWorkFlow", ConversionStrategy.Integer)]
	public int? IndexWorkflow {get;set;}
	  }
  [EntityApiInfo(CreateCommand = "", ListCommand = "listCategories", Root = "categories", Element = "category")]
  public partial class Category : FogBugzEntityBase
  {
    public Category() {}
	public Category(IDictionary<string,string> fields) : base(fields) {  }

		[PropertyMap("fDeleted", ConversionStrategy.Boolean)]
	public bool Deleted {get;set;}
		[PropertyMap("fScheduleItem", ConversionStrategy.Boolean)]
	public bool ScheduleItem {get;set;}
		[PropertyMap("iOrder", ConversionStrategy.Integer)]
	public int? Order {get;set;}
		[PropertyMap("ixCategory", ConversionStrategy.Integer)]
	public int? Index {get;set;}
		[PropertyMap("ixAttachmentIcon", ConversionStrategy.Integer)]
	public int? IndexAttachementIcon {get;set;}
		[PropertyMap("ixStatusDefault", ConversionStrategy.Integer)]
	public int? IndexStatusDefault {get;set;}
		[PropertyMap("nIconType", ConversionStrategy.Integer)]
	public int? IconIndex {get;set;}
		[PropertyMap("sCategory", ConversionStrategy.String)]
	public string Name {get;set;}
		[PropertyMap("sPlural", ConversionStrategy.String)]
	public string NamePlural {get;set;}
	  }
  [EntityApiInfo(CreateCommand = "new", ListCommand = "", Root = "cases", Element = "case")]
  public partial class Case : FogBugzEntityBase
  {
    public Case() {}
	public Case(IDictionary<string,string> fields) : base(fields) {  }

		[PropertyMap("ixBug", ConversionStrategy.Integer)]
	public int? Index {get;set;}
		[PropertyMap("ixParent", ConversionStrategy.Integer)]
	public int? IndexParent {get;set;}
		[PropertyMap("ixProject", ConversionStrategy.Integer)]
	public int? IndexProject {get;set;}
		[PropertyMap("ixArea", ConversionStrategy.Integer)]
	public int? IndexArea {get;set;}
		[PropertyMap("ixStatus", ConversionStrategy.Integer)]
	public int? IndexStatus {get;set;}
		[PropertyMap("ixPriority", ConversionStrategy.Integer)]
	public int? IndexPriority {get;set;}
		[PropertyMap("ixFixFor", ConversionStrategy.Integer)]
	public int? IndexFixFor {get;set;}
		[PropertyMap("ixCategory", ConversionStrategy.Integer)]
	public int? IndexCategory {get;set;}
		[PropertyMap("ixPersonAssignedTo", ConversionStrategy.Integer)]
	public int? IndexPersonAssignedTo {get;set;}
		[PropertyMap("ixPersonOpenedBy", ConversionStrategy.Integer)]
	public int? IndexPersonOpenedBy {get;set;}
		[PropertyMap("ixPersonResolvedBy", ConversionStrategy.Integer)]
	public int? IndexPersonResolvedBy {get;set;}
		[PropertyMap("ixPersonClosedBy", ConversionStrategy.Integer)]
	public int? IndexPersonClosedBy {get;set;}
		[PropertyMap("ixPersonLastEditedBy", ConversionStrategy.Integer)]
	public int? IndexPersonLastEditedBy {get;set;}
		[PropertyMap("sTitle", ConversionStrategy.String)]
	public string Title {get;set;}
		[PropertyMap("sFixFor", ConversionStrategy.String)]
	public string FixForName {get;set;}
		[PropertyMap("sArea", ConversionStrategy.String)]
	public string AreaName {get;set;}
		[PropertyMap("sProject", ConversionStrategy.String)]
	public string ProjectName {get;set;}
		[PropertyMap("sPriority", ConversionStrategy.String)]
	public string PriorityName {get;set;}
		[PropertyMap("hrsOrigEst", ConversionStrategy.Decimal)]
	public decimal? HoursOriginalEstimate {get;set;}
		[PropertyMap("hrsCurrEst", ConversionStrategy.Decimal)]
	public decimal? HoursCurrentEstimate {get;set;}
		[PropertyMap("hrsElapsed", ConversionStrategy.Decimal)]
	public decimal? HoursElapsed {get;set;}
		[PropertyMap("plugin_customfields_at_fogcreek_com_testxestimatexxinxminutesxu5a", ConversionStrategy.Integer)]
	public int? TestEstimate {get;set;}
		[PropertyMap("dtOpened", ConversionStrategy.DateTime)]
	public DateTime? DateOpened {get;set;}
		[PropertyMap("dtResolved", ConversionStrategy.DateTime)]
	public DateTime? DateResolved {get;set;}
		[PropertyMap("dtClosed", ConversionStrategy.DateTime)]
	public DateTime? DateClosed {get;set;}
		[PropertyMap("dtDue", ConversionStrategy.DateTime)]
	public DateTime? DateDue {get;set;}
		[PropertyMap("dlLastUpdated", ConversionStrategy.DateTime)]
	public DateTime? DateLastUpdated {get;set;}
		[PropertyMap("ixBugEventLatest", ConversionStrategy.Integer)]
	public int? IndexBugLastEvent {get;set;}
		[PropertyMap("ixBugEventLastView", ConversionStrategy.Integer)]
	public int? IndexBugEventLastView {get;set;}
		[PropertyMap("ixBugChildren", ConversionStrategy.IntegerList)]
	public IEnumerable<int> IndexBugChildren {get;set;}
		[PropertyMap("Operations", ConversionStrategy.StringList)]
	public List<string> Operations {get;set;}
		[PropertyMap("sTags", ConversionStrategy.StringList)]
	public List<string> Tags {get;set;}
		[PropertyMap("fOpen", ConversionStrategy.Boolean)]
	public bool IsOpen {get;set;}
		[PropertyMap("plugin_customfields_at_fogcreek_com_wikixnumberq6b", ConversionStrategy.String)]
	public string WikiPageId {get;set;}
	  }
}
