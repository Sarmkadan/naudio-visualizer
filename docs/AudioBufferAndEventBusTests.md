# AudioBufferAndEventBusTests

Unit tests for the `AudioBuffer` and `EventBus` components in the naudio-visualizer project. These tests validate the behavior of sample buffering, event dispatching, and subscription management under various conditions, including capacity constraints, concurrency, and lifecycle events.

## API

### `public void Write_SamplesWritten_CountIncreasesAccordingly()`

Validates that writing samples to the `AudioBuffer` increments the sample count by the exact number of samples written. The test asserts that the internal counter reflects the cumulative total after each write operation.

### `public void Read_SamplesConsumed_CountDecreasesAndDataReturned()`

Ensures that reading samples from the `AudioBuffer` reduces the sample count by the number of samples consumed and returns the expected data. The test verifies both the count adjustment and the integrity of the returned sample array.

### `public void Peek_DoesNotConsumeSamples_CountRemainsUnchanged()`

Confirms that calling `Peek` on the `AudioBuffer` does not alter the internal state or sample count. The test asserts that the count remains unchanged after the operation.

### `public void Write_ExceedsCapacity_OldestSamplesAreOverwritten()`

Checks that when the `AudioBuffer` exceeds its capacity, the oldest samples are overwritten in a FIFO manner. The test validates that the buffer retains only the most recent samples up to its capacity.

### `public void Clear_AfterWriting_BufferReturnsEmpty()`

Verifies that invoking `Clear` on the `AudioBuffer` resets the internal state such that subsequent reads return no data and the sample count is zero.

### `public void GetDurationSeconds_KnownSampleCountAndRate_ReturnsCorrectDuration()`

Tests that `GetDurationSeconds` calculates the correct duration in seconds given a known sample count and sample rate. The test asserts the returned value matches the expected duration.

### `public void Read_InsufficientSamples_ReturnsZeroPaddedAndCorrectActualSamplesRead()`

Ensures that reading more samples than are available from the `AudioBuffer` returns a zero-padded array of the requested size and reports the actual number of samples read via an out parameter.

### `public void Set_ThenTryGetValue_ReturnsStoredValue()`

Validates that storing a value in a key-value store and then retrieving it via `TryGetValue` returns the stored value. The test asserts the retrieved value matches the stored value.

### `public void Remove_ExistingKey_ReturnsTrueAndEntryIsGone()`

Confirms that removing an existing key from a key-value store returns `true` and that subsequent attempts to retrieve the key fail. The test asserts the entry is no longer present.

### `public void TryGetValue_ExpiredEntry_ReturnsFalse()`

Ensures that attempting to retrieve a value for an expired key in a key-value store returns `false`. The test validates that expired entries are not accessible.

### `public void GetStatistics_AfterAddingEntries_ReflectsCurrentSize()`

Checks that `GetStatistics` returns accurate metrics reflecting the current size and state of the key-value store after adding entries. The test asserts the statistics reflect the expected values.

### `public void Publish_WithRegisteredSubscriber_HandlerIsInvokedOnce()`

Validates that publishing an event to the `EventBus` invokes the registered subscriber's handler exactly once. The test asserts the handler is called with the expected payload.

### `public void Publish_AfterUnsubscribeAll_HandlerIsNeverInvoked()`

Ensures that after unsubscribing all handlers from the `EventBus`, publishing an event does not invoke any handlers. The test asserts no handlers are triggered.

### `public void GetSubscriberCount_AfterTwoSubscribes_ReturnsTwo()`

Confirms that after subscribing two handlers to the `EventBus`, `GetSubscriberCount` returns the correct count of two. The test validates the count reflects the number of active subscriptions.

### `public void Publish_NoSubscribersRegistered_DoesNotThrow()`

Tests that publishing an event when no subscribers are registered does not throw an exception. The test asserts the operation completes without error.

### `public void Subscribe_DisposeToken_RemovesSubscription()`

Validates that disposing the subscription token returned by `Subscribe` removes the associated handler from the `EventBus`. The test asserts the handler is no longer invoked after disposal.

## Usage

### Example 1: Testing AudioBuffer with Sample Writes and Reads
