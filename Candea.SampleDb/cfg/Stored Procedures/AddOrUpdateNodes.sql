CREATE PROCEDURE [cfg].[AddOrUpdateNodes] 
	@nodes cfg.NodeType READONLY
AS
BEGIN

	SET NOCOUNT ON;

	merge cfg.[Node] t
	using @nodes s
	on (t.[Id] = s.Id)
	when not matched by target
		then insert ([Name], [Value], [CreatedBy])
		values (s.[Name], s.[Value], s.CreatedBy)
	when matched
		then update set
			t.[Name] = s.[Name],
			t.[Value] = s.[Value],
			t.UpdatedBy = s.UpdatedBy,
			t.UpdatedDate = getDate();

END
