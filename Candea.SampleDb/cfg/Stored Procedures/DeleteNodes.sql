CREATE PROCEDURE cfg.DeleteNodes
	@nodeIds dbo.IntVal READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    delete n 
	from cfg.Node n
	inner join @nodeIds i
		on i.Id = n.Id;
END
