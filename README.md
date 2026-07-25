# Etch
### "Etch" is a tentative name.
Etch is a UI framework that strives to offer **a simple and direct public interface**.
It is **retained mode**, meaning updates don't happen unconditionally every frame, but only when something actually changed.
It is also **reactive** and **event-driven**, meaning dirty tracking doesn't happen through invalidation and tree navigation, but through events firing and delegates updating the view.

## To do
- [X] Property (fundamental class driving the entire reactive graph)
- [ ] Combiner (or any other system to bind to multiple source Property)
- [X] Skia on Silk.NET platform (cross-platform backend)
- [ ] Widgets (that are actually useful, rather than a red box)
