import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChatRoom1Component } from './chat-room1.component';

describe('ChatRoom1Component', () => {
  let component: ChatRoom1Component;
  let fixture: ComponentFixture<ChatRoom1Component>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChatRoom1Component ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChatRoom1Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
