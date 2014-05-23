In order to ensure that the model structure corresponds to the underlying json, the best approach is to proceed as follows (this is not essential but helpful especially for complex models):

1. Create the archetype in Umbraco
1. Create some sample data and publish
1. Get the json from umbraco.config, and remove meta-data
1. Run the following batch of tests (pseudo code)

```csharp
SetUp:
var Json_Test_String = @"...";
var testModel = new TestModel{Populated};

Test 1 (Model serializes correctly):
var result = JsonConvert.SerializeObject(testModel , Formatting.Indented);
Assert.AreEqual(Json_Test_String , result);

Test 2 (Model generates legitimate Archetype Json)
var converter = new ArchetypeValueConverter();
var json = JsonConvert.SerializeObject(model);
var archetype = converter.ConvertDataToSource(null, json, false);
Assert.NotNull(archetype);

Test 3 (json deserializes correctly)
var result = JsonConvert.DeserializeObject<TestModel>(Json_Test_String );
Assert.NotNull(result);
Assert.IsInstanceOf<TestModel>(result);
...Test correct property assignments

Test 4 (Model serializes and then deserializes correctly)
var json = JsonConvert.SerializeObject(testModel );
var result = JsonConvert.DeserializeObject<TestModel>(json);
Assert.NotNull(result);
Assert.IsInstanceOf<TestModel>(result);
...Test correct property assignments

```

