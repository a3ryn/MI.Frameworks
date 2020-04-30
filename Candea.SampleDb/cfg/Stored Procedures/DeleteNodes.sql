CREATE PROCEDURE cfg.DeleteNodes
	@nodeIds dbo.IntVal READONLY,
	@user varchar(10) --added as an exmaple for calling st proc with mixed input types; if null >> won't delete
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if @user is not null
		delete n 
		from cfg.Node n
		inner join @nodeIds i
			on i.Id = n.Id;
END
