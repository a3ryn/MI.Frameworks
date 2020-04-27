CREATE PROCEDURE [cfg].[AddNode] 
	@nodeName varchar(50),
	@nodeVal varchar(2000),
	@user varchar(10)
AS
BEGIN

	SET NOCOUNT ON;

	insert into cfg.Node
	(Name, Value, CreatedBy)
	values
	(@nodeName, @nodeVal, @user);

END
